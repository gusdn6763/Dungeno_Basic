using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerUi playerUi;
    [SerializeField] private PlayerPartUi partUi;

    [Header("턴 제한 시간")]
    [SerializeField] private float playerTurnTime = 0;
    public float PlayerTurnTime { get => playerTurnTime; }

    public int multiAttackCount = 0;
    public int AllowAttackCount { get => multiAttackCount; set => multiAttackCount = value; }
    public int CurrentAttackType { get; private set; }
    public string CurrentLocation { get; set; }

    public Action<bool> OnPlayerCanAttackEvent;

    private bool playerCanAttack;
    public bool PlayerCanAttack
    {
        get => playerCanAttack;
        set
        {
            playerCanAttack = value;
            OnPlayerCanAttackEvent?.Invoke(value);
        }
    }

    public float PlayerDamege { get => player.Damage; }

    public void Init()
    {
        CurrentLocation = "던전";
        playerCanAttack = true;

        playerUi.Init(player);
        partUi.Init(player);
    }

    public void SelectAttackType(int number)
    {
        CurrentAttackType = number;
    }
}
