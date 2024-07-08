using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public List<DungeonArea> dungeonAreas = new List<DungeonArea>();

    [SerializeField] private Area startArea;
    public Area CurrentArea { get; set; }

    public void Init()
    {
        CurrentArea = startArea;

        for (int i = 0; i < dungeonAreas.Count; i++)
        {
            DungeonArea dungeonArea = dungeonAreas[i];

            Managers.Game.onStateChange += dungeonArea.ExplorEnd;
        }
    }

    public string GetAreaName()
    {
        return CurrentArea.AreaName;
    }
}
