using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DungeonArea area;
    [SerializeField] private Button startButton;

    public void Init()
    {
        startButton.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        area.ExplorEnd();
    }
}
