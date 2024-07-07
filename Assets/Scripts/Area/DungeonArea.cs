using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DungeonArea : Area
{
    [Header("Ž�� ī��Ʈ �ð� ������")]
    [SerializeField] private float advantureCountAddTime;

    [Header("������Ʈ ���� ���� Ȯ��")]
    public SerializableDictionary<int, float> spawnValueDic;

    [Header("������Ʈ ���� Ȯ��")]
    public SerializableDictionary<InteractionObject, float> InteractionObjDic;

    public int FightMonsterLevel { get; set; }

    #region ���� ��ȭ�� 1ȸ ����
    public override void StateEnter()
    {
        switch (Managers.Game.CurrentGameState)
        {
            case E_GameState.Explor_End:
                ExplorEnd();
                break;
            case E_GameState.Battle_Start:
                break;
        }
        
    }

    public void ExplorEnd()
    {
        int nCount = GetSpawnCount();

        Debug.Log("���� ���� : " + nCount);

        for (int i = 0; i < nCount; i++)
            SpawnInteractionObject();

        CheckMonsterDetection();

        if(HaveStateMonster(E_MonsterState.Battle))
        {
            BattleStart();         
        }
        else
        {
            UpdateObjectLifetimes();
            Managers.Game.GameStateEnter(E_GameState.Exploring);
        }
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
                        (spawnObject as Creature).OnDamagedEvent += PlayerMonsterAttack;

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

    #endregion

    #region ���� ����
    public void PlayerMonsterAttack(int creatureLevel)
    {
        FightMonsterLevel = creatureLevel;

        if (HaveStateMonster(E_MonsterState.Battle))
            BattleStart();
    }
    public bool HaveStateMonster(E_MonsterState monsterState)
    {
        List<Creature> allMonsters = FindTypeObjects<Creature>();
        for (int i = 0; i < allMonsters.Count; i++)
        {
            Creature creature = allMonsters[i];

            if (creature.CurrentState == monsterState)
                return true;
        }
        return false;
    }
    public void BattleStart()
    {
        SetHighLevelMonsterBehavior();
        SetMidLevelMonsterBehavior();
        SetLowLevelMonsterBehavior();
        Managers.Game.GameStateEnter(E_GameState.Battle_Start);
    }

    protected override void HandleOnDead(InteractionObject obj)
    {
        base.HandleOnDead(obj);

        if(HaveStateMonster(E_MonsterState.Battle))
        {

        }
        else if (HaveStateMonster(E_MonsterState.BattleWait))
        {

        }
    }
    #endregion
    public void UpdateObjectLifetimes()
    {
        for (int i = 0; i < interactionObjects.Count; i++)
            interactionObjects[i].UpdateLifetime();
    }
}


