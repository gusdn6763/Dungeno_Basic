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

    [Header("�÷��̾� �� �ð�")]
    [SerializeField] private float playerTurnTime = 0;
    public float PlayerTurnTime { get => playerTurnTime; }
    public Area CurrentArea { get; set; }

    public void Init()
    {
        elaspedTime = playerTurnTime;

        playerUi.Init(player);
        partUi.Init(player);
    }

    #region �ǽð� ����
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
            //���� ���ɿ��� ��ħ
            if (Managers.Game.CurrentGameState == E_GameState.Battle_Start)
            {
                PlayerAttack();
            }
            else
            {
                //���� ����
                if(canAttackCount == 0 && AttackList.Count != 0)
                    PlayerAttack();
                else
                    elaspedTime = playerTurnTime;
            }
        }

        //����׿�
        {
            text.text = (PlayerTurnTime - elaspedTime).ToString("F2");
        }
    }
    #endregion

    #region �÷��̾� ����

    public Action<bool> CanSelectEvent;     //���� ���� ���� ���� ����

    private int canAttackCount = 0;
    private bool partCanSelect = false;
    public bool PartCanSelect
    {
        get => partCanSelect; 
        private set
        {
            partCanSelect = value;
            CanSelectEvent?.Invoke(partCanSelect);
        }    //���� Ÿ�� ���� ����
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

        //List<Creature> creatures = Managers.Battle.GetStateMonster(E_MonsterState.Battle);
        List<Creature> creatures = Managers.Battle.allCreature;

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

    public void Damaged(float damage)
    {
        player.Damaged(damage);
        partUi.StatusUpdate();
        StartCoroutine(ShowBloodScreen());
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

    public void SelectAttackPart(Part part)
    {
        canAttackCount--;
        AttackList.Add(part);

        if (canAttackCount == 0)
            PartCanSelect = false;
    }

    #endregion

    [SerializeField] private Image bloodScreen;
    IEnumerator ShowBloodScreen()
    {
        //BloodScreen �ؽ�ó�� ���İ��� �ұ�Ģ�ϰ� ����
        bloodScreen.color = new Color(1, 0, 0, UnityEngine.Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen �ؽ�ó�� ������ ��� 0���� ����
        bloodScreen.color = Color.clear;
    }
}
