using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DungeonArea : Area
{
    [Header("���� ���� ����")]
    [SerializeField] E_GameState currentE_GameState;

    [Header("Ž�� ī��Ʈ �ð�")]
    [SerializeField] private float advantureCountTime;

    [Header("Ȯ�ο�-����ð�")]
    [SerializeField] private float currentAdvantureTime = 0f;

    [Header("Ž�� ī��Ʈ �ð� ������")]
    [SerializeField] private float advantureCountAddTime;

    [Header("������Ʈ ���� ���� Ȯ��")]
    public SerializableDictionary<int, float> spawnValueDic;

    [Header("������Ʈ ���� Ȯ��")]
    public SerializableDictionary<InteractionObject, float> InteractionObjDic;

    public int FightMonsterLevel { get; set; }

    #region Update �ǽð� ����
    void Update()
    {
        //����Ŭ�� �Ǵ� �����̽��ٽ� ���� ���¸� ����
        HandleInput();

        //���� ���º� ����
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

    #region ���� ��ȭ�� 1ȸ ����
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

        Debug.Log("���� ���� : " + nCount);

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

    #region ����Ŭ��

    [Header("����Ŭ�� ���ѽð�")]
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

    #region ���� �� ����
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

                    Debug.Log("���� : " + spawnObject.commandName + "Ȯ��:" + randomValue);
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

        // �� �� ���� Ÿ���� ��� null ��ȯ
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
        Debug.Log("���� ����");
        return false;
    }

    #endregion

    #region ���� �� ����
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

                //0�� ��� ����
                Debug.Log("��� ����,���� ����,������,����");
                if (value == 0)     //��� ����
                    FightMonsterLevel = creature.Index;
                if (value == 1)     //���� ����
                    creature.CurrentState = E_MonsterState.BattleWait;
                if (value == 2)     //������
                {
                }
                if (value == 3)     //����
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

    #region ���� ����

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


