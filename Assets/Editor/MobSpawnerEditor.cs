using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MobSpawner))]
public class MobSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        MobSpawner mobSpawner = (MobSpawner)target;

        // Calculate the total spawn percentage
        float totalSpawnPercent = 0f;
        foreach (MobSpawnProbability mobSpawn in mobSpawner.m_MobList)
        {
            totalSpawnPercent += mobSpawn.spawnPercent;
        }

        // Display the total spawn percentage
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Total Spawn Percentage: " + totalSpawnPercent + "%");

        // Check if the total spawn percentage exceeds 100%
        if (totalSpawnPercent > 100f)
        {
            EditorGUILayout.HelpBox("Total spawn percentage exceeds 100%. Please adjust the values.", MessageType.Error);
        }
    }
}