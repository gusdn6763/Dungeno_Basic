using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DungeonArea : Area
{
    [Header("������Ʈ ���� ���� Ȯ��")]
    public SerializableDictionary<int, float> spawnValueDic;

    [Header("������Ʈ ���� Ȯ��")]
    public SerializableDictionary<InteractionObject, float> InteractionObjDic;

    public void ExplorEnd(E_AreaState gameState)
    {
        if (gameState == E_AreaState.Explor_End)
        {
            int nCount = GetSpawnCount();

            Debug.Log("���� ���� : " + nCount);

            for (int i = 0; i < nCount; i++)
                SpawnInteractionObject();

            Managers.Battle.CheckMonsterDetection();

            if (Managers.Game.CurrentGameState != E_AreaState.Battle_Start)
            {
                UpdateObjectLifetimes();
                Managers.Game.GameStateEnter(E_AreaState.Exploring);
            }
        }
    }

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

                    interactionObjects.Add(spawnObject);

                    Debug.Log("���� : " + spawnObject.commandName);
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

    public void UpdateObjectLifetimes()
    {
        for (int i = 0; i < interactionObjects.Count; i++)
            interactionObjects[i].UpdateLifetime();
    }
}


