using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerUi playerUi;
    [SerializeField] private PlayerPartUi partUi;

    [Header("플레이어 턴 시간")]
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

    public void Damaged(E_PartType partType, float damage)
    {
        player.Damaged(partType, damage);
    }

    public void PlayerAttack(PartButton button)
    {
        if (playerCanAttack)
        {
            button.Damaged(player.Damage);
            playerCanAttack = false;
        }
    }


    IEnumerator PlayerTurnCoroutine()
    {

    }
}
