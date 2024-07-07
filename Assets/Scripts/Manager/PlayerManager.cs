using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerUi playerUi;
    [SerializeField] private PlayerPartUi partUi;
    [SerializeField] private Image playerCoolTime;

    [Header("플레이어 턴 시간")]
    [SerializeField] private float playerTurnTime = 0;
    public float PlayerTurnTime { get => playerTurnTime; }
    public string CurrentLocation { get; set; }

    public void Init()
    {
        CurrentLocation = "던전";
        elaspedTime = playerTurnTime;

        playerUi.Init(player);
        partUi.Init(player);
    }

    #region 실시간 실행
    private void Update()
    {
        ProcessPlayerBattleTurn();
    }

    [SerializeField] TextMeshProUGUI text;
    private float elaspedTime = 0;
    public void ProcessPlayerBattleTurn()
    {
        elaspedTime += Time.deltaTime;
        playerCoolTime.fillAmount = elaspedTime / playerTurnTime;

        if (elaspedTime >= PlayerTurnTime)
        {
            //공격 가능에도 놓침
            if (Managers.Game.CurrentGameState == E_GameState.Battle_Start)
            {
                PlayerAttack();
            }
            else
            {
                //선제 공격
                if(canAttackCount == 0 && AttackList.Count != 0)
                    PlayerAttack();
                else
                    elaspedTime = playerTurnTime;
            }
        }

        //디버그용
        {
            text.text = (PlayerTurnTime - elaspedTime).ToString("F2");
        }
    }
    #endregion

    #region 플레이어 공격

    public Action<bool> CanSelectEvent;     //몬스터 부위 공격 가능 여부

    private int canAttackCount = 0;
    private bool partCanSelect = false;
    public bool PartCanSelect
    {
        get => partCanSelect; 
        private set
        {
            partCanSelect = value;
            CanSelectEvent?.Invoke(partCanSelect);
        }    //공격 타입 선택 여부
    }
    public void SelectAttackCount(int number)
    {
        AttackList.Clear();
        PartCanSelect = true;
        canAttackCount = number;
    }

    public void SelectFireWallAttack()
    {
        AttackList.Clear();
        PartCanSelect = false;
        canAttackCount = 0;

        List<Creature> creatures = Managers.Game.CurrentArea.FindTypeObjects<Creature>();
        for (int i = 0; i < creatures.Count; i++)
        {
            bool getPart = false;
            Creature creature = creatures[i];
            if (creature.HavePart(E_PartType.LeftLeg))
            {
                AttackList.Add(creature.GetPart(E_PartType.LeftLeg));
                getPart = true;
            }
            if (creature.HavePart(E_PartType.RightLeg))
            {
                AttackList.Add(creature.GetPart(E_PartType.RightLeg));
                getPart = true;
            }

            if (getPart == false)
                if (creature.HavePart(E_PartType.Chest))
                    AttackList.Add(creature.GetPart(E_PartType.Chest));
        }
    }

    public void Damaged(E_PartType partType, float damage)
    {
        player.Damaged(partType, damage);
    }

    public List<Part> AttackList { get; set; } = new List<Part>();
    public void PlayerAttack()
    {
        elaspedTime = 0;
        PartCanSelect = false;

        for (int i = 0; i < AttackList.Count; i++)
        {
            Part part = AttackList[i];
            part.Damaged(player.Damage);
        }

        AttackList.Clear();  
    }

    public void SelectAttackPart(PartButton partButton)
    {
        canAttackCount--;
        AttackList.Add(partButton.CurrentPart);

        if (canAttackCount == 0)
            PartCanSelect = false;
    }

    #endregion
}
