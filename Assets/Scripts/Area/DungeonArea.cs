using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using static Define;

public class DungeonArea : Area
{
    [Header("오브젝트 스폰 갯수 확률")]
    public SerializableDictionary<int, float> spawnValueDic;

    [Header("오브젝트 스폰 확률")]
    public SerializableDictionary<InteractionObject, float> InteractionObjDic;

    private List<Creature> creatures = new List<Creature>();
    public int FightMonsterLevel { get; set; }
    public override void Init()
    {
        Managers.Input.keyAction -= OnKeyboard;
        Managers.Input.keyAction += OnKeyboard;
    }

    private void Update()
    {
        HandleUIInteraction();
    }

    #region 던전 몹 생성
    public void Spawn()
    {
        int nCount = GetSpawnCount();

        Debug.Log("스폰 갯수 : " + nCount);

        for (int i = 0; i < nCount; i++)
            SpawnInteractionObject();

        if(currentSelectedCreature == null)
            SelectClosestCreatureToCenter();

        UpdateCreatureSelection();
    }
    public int GetSpawnCount()
    {
        float randomValue = UnityEngine.Random.Range(0, 100f);
        float accumulatedProbability = 0f;
        foreach (SerializableDictionary<int, float>.Pair spawnValue in spawnValueDic)
        {
            accumulatedProbability += spawnValue.Value;

            if (randomValue <= accumulatedProbability)
                return spawnValue.Key;
        }
        return 0;
    }
    public void SpawnInteractionObject()
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);

        float accumulatedProbability = 0f;
        foreach (SerializableDictionary<InteractionObject, float>.Pair gameEntity in InteractionObjDic)
        {
            accumulatedProbability += gameEntity.Value;

            if (randomValue <= accumulatedProbability)
            {
                Vector3 spawnPosition = Vector3.zero;
                if (CanSpawn(gameEntity.Key, out spawnPosition))
                {
                    InteractionObject spawnObject = CreateObjectByType(gameEntity.Key);

                    spawnObject.Spawn();
                    spawnObject.transform.position = spawnPosition;
                    spawnObject.CurrentArea = this;

                    interactionObjects.Add(spawnObject);

                    Debug.Log("생성 : " + spawnObject.commandName);
                }
                return ;
            }
        }
    }
    private InteractionObject CreateObjectByType(InteractionObject obj)
    {
        if (obj is Item)
        {
            return Managers.Object.Spawn<Item>(obj);
        }
        else if (obj is Creature)
        {
            Creature creature = Managers.Object.Spawn<Creature>(obj);

            creatures.Add(creature);
            return creature;
        }

        // 알 수 없는 타입의 경우 null 반환
        Debug.LogWarning("Unknown InteractionObject type");
        return null;
    }
    public bool CanSpawn(InteractionObject obj, out Vector3 spawnPosition)
    {
        Vector3 size = obj.GetSize();

        spawnPosition = Vector3.zero;

        for (int i = 0; i < 1000; i++)
        {
            spawnPosition = Utils.GetRandomPosition(size);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(spawnPosition, size, 0);

            if (colliders.Length == 0)
                return true;
        }
        Debug.Log("생성 실패");
        return false;
    }

    #endregion

    public override void AreaStateEnter(E_AreaState areaState)
    {
        switch (areaState)
        {
            case E_AreaState.Explore_End:
                Spawn();
                CheckDetection();
                break;
            case E_AreaState.Battle_Start:
                BattleStart();
                UpdateCreatureSelection();
                break;
            case E_AreaState.Battle_End:
                Managers.Game.CameraZoomOff();
                break;
        }
    }

    public void CheckDetection()
    {
        for (int i = 0; i < interactionObjects.Count; i++)
        {
            InteractionObject interactionObject = interactionObjects[i];
            if (interactionObject.Detect())
                interactionObject.DetectAction();
            else
                interactionObject.UpdateLifeTime();
        }
    }

    #region 던전 전투 관련
    public void BattleStart()
    {
        FightMonsterLevel = CurrentMaxLevel();

        SetHighLevelMonsterBehavior();
        SetMidLevelMonsterBehavior();
        SetLowLevelMonsterBehavior();
    }
    public int CurrentMaxLevel()
    {
        int idx = 0;
        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i].CurrentState == E_MonsterState.Battle)
                idx = Mathf.Max(idx, creatures[i].Index);
        }
        return idx;
    }
    public void SetHighLevelMonsterBehavior()
    {
        foreach (var creature in creatures)
        {
            if (creature.Index > FightMonsterLevel)
            {
                int value = UnityEngine.Random.Range(0, 4);
                switch (value)
                {
                    case 0: // 즉시 참여
                        FightMonsterLevel = creature.Index;
                        break;
                    case 1: // 연속 참여
                        creature.SetState(E_MonsterState.BattleWait);
                        break;
                    case 2: // 무관심
                        break;
                    case 3: // 도망
                        creature.SetState(E_MonsterState.Run);
                        break;
                }
            }
        }
    }
    public void SetMidLevelMonsterBehavior()
    {
        foreach (var creature in creatures)
        {
            if (creature.Index == FightMonsterLevel)
                creature.SetState(E_MonsterState.Battle);
        }
    }
    public void SetLowLevelMonsterBehavior()
    {
        foreach (var creature in creatures)
        {
            if (creature.Index < FightMonsterLevel)
                creature.SetState(E_MonsterState.Run);
        }
    }
    #endregion

    #region UI관련
    [SerializeField] private SpriteRenderer selectorSprite;
    private CreaturePartUi currentPartUi;
    private Creature currentSelectedCreature;
    private bool isUiLockedByEnter = false;
    private Vector2 lastMousePosition;
    public void SelectClosestCreatureToCenter()
    {
        if (creatures.Count > 0)
            currentSelectedCreature = creatures.OrderBy(c => Vector2.Distance(Vector3.zero, c.transform.position)).First();
    }
    public void SelectLeftCreature()
    {
        if (creatures.Count == 0) 
            return;

        if (currentSelectedCreature == null)
        {
            SelectClosestCreatureToCenter();
            //currentSelectedCreature = creatures.OrderBy(c => c.transform.position.x).Last();
        }
        else
        {
            Vector2 currentPos = currentSelectedCreature.transform.position;
            var leftCreatures = creatures.Where(c => c.transform.position.x < currentPos.x).ToList();

            if (leftCreatures.Count > 0)
            {
                currentSelectedCreature = leftCreatures.OrderByDescending(c => c.transform.position.x).First();
            }
            else
            {
                currentSelectedCreature = creatures.OrderByDescending(c => c.transform.position.x).First();
            }
        }
    }
    public void SelectRightCreature()
    {
        if (creatures.Count == 0) 
            return;

        if (currentSelectedCreature == null)
        {
            SelectClosestCreatureToCenter();
            //currentSelectedCreature = creatures.OrderBy(c => c.transform.position.x).First();
        }
        else
        {
            Vector2 currentPos = currentSelectedCreature.transform.position;
            var rightCreatures = creatures.Where(c => c.transform.position.x > currentPos.x).ToList();

            if (rightCreatures.Count > 0)
            {
                currentSelectedCreature = rightCreatures.OrderBy(c => c.transform.position.x).First();
            }
            else
            {
                currentSelectedCreature = creatures.OrderBy(c => c.transform.position.x).First();
            }
        }
    }
    private void UpdateCreatureSelection()
    {
        if (currentSelectedCreature)
        {
            selectorSprite.transform.SetParent(currentSelectedCreature.transform);
            selectorSprite.transform.position = currentSelectedCreature.transform.position + new Vector3(0, 1, 0);
            selectorSprite.enabled = true;
            if(Managers.Area.AreaState == E_AreaState.Battle_Start)
                Managers.Game.CameraZoomOn(currentSelectedCreature.transform.position);
        }
        else
        {
            selectorSprite.transform.SetParent(null);
            selectorSprite.enabled = false;
        }
    }

    private void HandleUIInteraction()
    {
        Vector2 mousePosition = Input.mousePosition;

        // 마우스가 움직였는지 확인
        if (mousePosition != lastMousePosition)
        {
            isUiLockedByEnter = false;
            lastMousePosition = mousePosition;
        }

        if (!isUiLockedByEnter)
        {
            CreaturePartUi hoveredUi = null;

            // 모든 생물체의 UI를 검사
            foreach (var creature in creatures)
            {
                if (creature.PartUi != null && IsPointerOverUI(creature.PartUi, mousePosition))
                {
                    hoveredUi = creature.PartUi;
                    break;
                }
            }

            // 현재 호버된 UI 업데이트
            if (hoveredUi != currentPartUi)
            {
                if (currentPartUi != null)
                    currentPartUi.OpenClose(false);

                currentPartUi = hoveredUi;

                if (currentPartUi != null)
                    currentPartUi.OpenClose(true);
            }
        }
    }
    private bool IsPointerOverUI(CreaturePartUi partUi, Vector2 screenPosition)
    {
        RectTransform rectTransform = partUi.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition, Camera.main);
    }


    #endregion

    #region 입력 관련
    void OnKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectLeftCreature();
            UpdateCreatureSelection();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectRightCreature();
            UpdateCreatureSelection();
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            if(currentSelectedCreature)
            {
                if (currentPartUi)
                    currentPartUi.OpenClose(false);

                currentPartUi = currentSelectedCreature.PartUi;
                isUiLockedByEnter = true;
                currentPartUi.OpenClose(true);
            }
        }
        else if (Input.GetKey(KeyCode.Backspace))
        {
            if (currentPartUi)
            {
                currentPartUi.OpenClose(false);
                currentPartUi = null;
                isUiLockedByEnter = true;
            }
        }
    }
    #endregion

    public override void DestoryArea(InteractionObject obj)
    {
        if (interactionObjects.Contains(obj))
            interactionObjects.Remove(obj);

        if (obj is Creature)
        {
            Creature creature = obj as Creature;
            if (creatures.Contains(creature))
                creatures.Remove(creature);

            if (currentSelectedCreature == creature)
            {
                currentSelectedCreature = null;
                SelectClosestCreatureToCenter();
                UpdateCreatureSelection();
            }
        }
    }
}


