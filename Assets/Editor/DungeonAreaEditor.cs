using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonArea))]
public class TextObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        DungeonArea dungeonArea = (DungeonArea)target;

        // Calculate the total spawn percentage
        float totalSpawnPercent = 0f;
        foreach (SerializableDictionary<InteractionObject, float>.Pair mobSpawn in dungeonArea.InteractionObjDic)
            totalSpawnPercent += mobSpawn.Value;

        float totalSpawnCountPercent = 0f;
        foreach (SerializableDictionary<int, float>.Pair mobSpawn in dungeonArea.spawnValueDic)
            totalSpawnCountPercent += mobSpawn.Value;

        // Display the total spawn percentage
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Total Spawn Percentage: " + totalSpawnPercent + "%");
        EditorGUILayout.LabelField("Total SpawnCount Percentage: " + totalSpawnCountPercent + "%");

        // Check if the total spawn percentage exceeds 100%
        if (totalSpawnPercent > 100f)
            EditorGUILayout.HelpBox("Total spawn percentage exceeds 100%. Please adjust the values.", MessageType.Error);

        if (totalSpawnCountPercent > 100f)
            EditorGUILayout.HelpBox("Total SpawnCount percentage exceeds 100%. Please adjust the values.", MessageType.Error);
    }
}