using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DungeonArea : Area
{
    [Header("현재 게임 상태")]
    [SerializeField] E_GameState currentE_GameState;

    [Header("탐험 카운트 시간")]
    [SerializeField] private float advantureCountTime;

    [Header("확인용-현재시간")]
    [SerializeField] private float currentAdvantureTime = 0f;

    [Header("탐험 카운트 시간 증가량")]
    [SerializeField] private float advantureCountAddTime;

    [Header("오브젝트 스폰 갯수 확률")]
    public SerializableDictionary<int, float> spawnValueDic;

    [Header("오브젝트 스폰 확률")]
    public SerializableDictionary<InteractionObject, float> InteractionObjDic;

    public int FightMonsterLevel { get; set; }

    #region Update 실시간 실행
    void Update()
    {
        //더블클릭 또는 스페이스바시 시작 상태를 변경
        HandleInput();

        //게임 상태별 진행
        GameStateUpdate();
    }

    public void GameStateUpdate()
    {
        switch (currentE_GameState)
        {
            case E_GameState.Exploring:
                ProcessExplorationTime();
                break;
            case E_GameState.Battle_Start:
                ProcessPlayerBattleTurn();
                break;
        }
    }
    
    public void ProcessExplorationTime()
    {
        currentAdvantureTime += Time.deltaTime;

        if (currentAdvantureTime >= advantureCountTime)
        {
            currentAdvantureTime = 0;
            GameStateEnter(E_GameState.Explor_End);
        }
    }

    private float playerTurnTime = 0;

    public void ProcessPlayerBattleTurn()
    {
        playerTurnTime += Time.deltaTime;

        if (playerTurnTime >= Managers.Player.PlayerTurnTime)
        {
            PlayerAttack();
        }
    }

    public void PlayerAttack()
    {
        playerTurnTime = 0;
        if (Managers.Player.CurrentAttackType == 1)
        {

        }
    }
    #endregion

    #region 상태 변화시 1회 실행
    public void GameStateEnter(E_GameState gameState)
    {
        if (gameState != currentE_GameState)
        {
            currentE_GameState = gameState;

            switch (currentE_GameState)
            {
                case E_GameState.Explor_End:
                    ExplorEnd();
                    break;
                case E_GameState.Battle_Start:
                    break;
            }
        }
    }

    public void ExplorEnd()
    {
        int nCount = GetSpawnCount();

        Debug.Log("스폰 갯수 : " + nCount);

        for (int i = 0; i < nCount; i++)
            SpawnInteractionObject();

        CheckMonsterDetection();

        if(CheckBattleStateMonster())
        {
            Battle();         
        }
        else
        {
            UpdateObjectLifetimes();
            GameStateEnter(E_GameState.Exploring);
        }
    }

    #endregion

    #region 더블클릭

    [Header("더블클릭 제한시간")]
    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;
    private void HandleInput()
    {
        if (CheckDoubleClick() || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentE_GameState == E_GameState.Exploring)
                GameStateEnter(E_GameState.Explore_Stop);
            else if (currentE_GameState == E_GameState.Explore_Stop)
                GameStateEnter(E_GameState.Exploring);
        }
    }

    public bool CheckDoubleClick()
    {
        if (m_IsOneClick && ((Time.time - m_Timer) > m_DoubleClickSecond))
        {
            m_IsOneClick = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!m_IsOneClick)
            {
                m_Timer = Time.time;
                m_IsOneClick = true;
            }
            else if (m_IsOneClick && ((Time.time - m_Timer) < m_DoubleClickSecond))
            {
                m_IsOneClick = false;
                return true;
            }
        }
        return false;
    }
    #endregion

    #region 던전 몹 생성
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
                    spawnObject.EventOnDead += HandleOnDead;

                    if (spawnObject is Creature) 
                        (spawnObject as Creature).OnDamagedEvent += PlayerFirstAttack;

                    interactionObjects.Add(spawnObject);

                    Debug.Log("생성 : " + spawnObject.commandName + "확률:" + randomValue);
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
            return Managers.Object.Spawn<Creature>(obj);
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

            if (Physics2D.OverlapBox(spawnPosition, size, 0) == false)
            {
                return true;
            }
        }
        Debug.Log("생성 실패");
        return false;
    }

    #endregion

    #region 던전 몹 감지
    public void CheckMonsterDetection()
    {
        List<Creature> allMonsters = FindTypeObjects<Creature>();
        for (int i = 0; i < allMonsters.Count; i++)
        {
            Creature creature = allMonsters[i];

            if (creature.Detect())
                creature.DetectAction();
        }
    }

    public void SetHighLevelMonsterBehavior()
    {
        List<Creature> allMonsters = FindTypeObjects<Creature>();

        for (int i = 0; i < allMonsters.Count; i++)
        {
            Creature creature = allMonsters[i];

            if (creature.Index > FightMonsterLevel)
            {
                int value = UnityEngine.Random.Range(0, 4);

                //0은 즉시 참여
                Debug.Log("즉시 참여,연속 참여,무관심,도망");
                if (value == 0)     //즉시 참여
                    FightMonsterLevel = creature.Index;
                if (value == 1)     //연속 참여
                    creature.CurrentState = E_MonsterState.BattleWait;
                if (value == 2)     //무관심
                {
                }
                if (value == 3)     //도망
                {
                    creature.CurrentState = E_MonsterState.Run;
                }
            }
        }
    }
    public void SetMidLevelMonsterBehavior()
    {
        List<Creature> allMonsters = FindTypeObjects<Creature>();

        for (int i = 0; i < allMonsters.Count; i++)
        {
            Creature creature = allMonsters[i];

            if (creature.Index == FightMonsterLevel)
                creature.CurrentState = E_MonsterState.Battle;
        }
    }
    public void SetLowLevelMonsterBehavior()
    {
        List<Creature> allMonsters = FindTypeObjects<Creature>();

        for (int i = 0; i < allMonsters.Count; i++)
        {
            Creature creature = allMonsters[i];

            if (creature.Index < FightMonsterLevel)
                creature.CurrentState = E_MonsterState.Run;
        }
    }

    public void PlayerFirstAttack(int creatureLevel)
    {
        FightMonsterLevel = creatureLevel;

        if (CheckBattleStateMonster())
            Battle();
    }

    #endregion

    #region 전투 관련

    public bool CheckBattleStateMonster()
    {
        List<Creature> allMonsters = FindTypeObjects<Creature>();
        for (int i = 0; i < allMonsters.Count; i++)
        {
            Creature creature = allMonsters[i];

            if (creature.CurrentState == E_MonsterState.Battle)
                return true;
        }
        return false;
    }

    public void Battle()
    {
        SetHighLevelMonsterBehavior();
        SetMidLevelMonsterBehavior();
        SetLowLevelMonsterBehavior();
        GameStateEnter(E_GameState.Battle_Start);
    }
    #endregion

    public void UpdateObjectLifetimes()
    {
        for (int i = 0; i < interactionObjects.Count; i++)
            interactionObjects[i].UpdateLifetime();
    }
}


