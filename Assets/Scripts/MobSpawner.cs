using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MobSpawnProbability
{
    public MobCommand command;
    [Header("»ý¼ºÈ®·ü"), Range(0, 100)]
    public float spawnPercent = 0;
}

public class MobSpawner : MonoBehaviour
{
    public List<MobSpawnProbability> m_MobList = new List<MobSpawnProbability>();

    [Header("»ý¼ºÈ®·ü")]
    public float m_SpawnChance;

    [Header("½ºÆù½Ã°£")]
    public float spawnTime;
    private float elapsedTime = 0f;

    public bool CanSpawn(float time)
    {
        elapsedTime += time;

        if (elapsedTime >= spawnTime)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= m_SpawnChance)
                return true;
        }

        return false;
    }

    public MobCommand SpawnMobCommand()
    {
        elapsedTime = 0f;
        float randomValue = Random.Range(0f, 100f);

        float accumulatedProbability = 0f;
        foreach (MobSpawnProbability mobSpawn in m_MobList)
        {
            accumulatedProbability += mobSpawn.spawnPercent;

            if (randomValue <= accumulatedProbability)
            {
                Debug.Log("»ý¼º : " + mobSpawn.command.commandName + "È®·ü:" + randomValue);
                return Instantiate(mobSpawn.command, new Vector3(9999, 9999, 9999), Quaternion.identity);
            }
        }
        
        return null;
    }
}
