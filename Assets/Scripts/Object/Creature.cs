using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class Creature : InteractionObject
{
    public Action<Creature> OnDestoryEvent;

    [Header("스텟")]
    [SerializeField] private Stat stat;

    [Header("체력")]
    [SerializeField] private float hp;
    public float Hp { get => hp; set => hp = value; }

    [Header("부위")]
    public List<CreaturePart> parts = new List<CreaturePart>();

    [Header("속도")]
    public float speed;

    [Header("공격 속도")]
    public float attackTime;

    [Header("공격력")]
    public float damage;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        for(int i = 0; i < parts.Count; i++)
        {
            CreaturePart creaturePart = parts[i];
            creaturePart.Init(this);
            creaturePart.OnDamaged += Damaged;
            creaturePart.OnBroken += BrokenPart;
        }

        partUi = GetComponentInChildren<CreaturePartUi>();
        return true;
    }
    public override void Spawn()
    {
        SetState(E_MonsterState.Idle);
        StartCoroutine(StateUpdate());
    }

    #region AI
    [Header("확인용-현재상태")]
    [SerializeField] protected E_MonsterState currentState = E_MonsterState.None;
    public E_MonsterState CurrentState { get => currentState; }

    #region 실시간
    protected virtual IEnumerator StateUpdate()
    {
        while (true)
        {
            switch (currentState)
            {
                case E_MonsterState.Notice:
                    UpdateNotice();
                    break;
                case E_MonsterState.Battle:
                    UpdateBattle();
                    break;
                case E_MonsterState.Run:
                    UpdateRun();
                    break;
            }

            yield return null;
        }
    }

    protected float noticeElaspedTime = 0; 
    protected void UpdateNotice()
    {
        noticeElaspedTime += Time.deltaTime;

        if (noticeElaspedTime >= detectionTime)
            NoticeAction();
    }

    [SerializeField] private Image coolTimeImage;
    [SerializeField] TextMeshProUGUI coolTimeText;
    private float elaspedTime = 0;
    public void UpdateBattle()
    {
        elaspedTime += Time.deltaTime;
        coolTimeImage.fillAmount = elaspedTime / attackTime;

        if (elaspedTime >= attackTime)
        {
            elaspedTime = 0;
            Managers.Player.Damaged(damage);
        }

        //디버그용
        {
            coolTimeText.text = (attackTime - elaspedTime).ToString("F2");
        }
    }
    protected virtual void UpdateRun()
    {
        transform.Translate(runDirection * speed);

        if(Managers.Game.CheckOutArea(transform.position))
            DestoryObject();
    }
    #endregion

    #region 상태 실행
    public void SetState(E_MonsterState monsterState)
    {
        if (currentState != monsterState)
        {
            currentState = monsterState;

            switch (monsterState)
            {
                case E_MonsterState.Notice:
                    EnterNotice();
                    break;
                case E_MonsterState.Battle:
                    EnterBattle();
                    break;
                case E_MonsterState.BattleWait:
                    EnterBattleWait();
                    break;
                case E_MonsterState.Run:
                    EnterRun();
                    break;
                case E_MonsterState.Dead:
                    DestoryObject();
                    break;
            }
        }
    }

    public void EnterNotice()
    {
        text.color = Color.gray;
    }

    private Vector2 runDirection;
    public void EnterRun()
    {
        text.color = Color.blue;
        Vector2 centerToObject = (Vector2)transform.position - (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        runDirection = centerToObject.normalized;
    }
    public void EnterBattle()
    {
        text.color = Color.red;
    }

    public void EnterBattleWait()
    {
        text.color = Color.yellow;
    }

    public override void DestoryObject()
    {
        OnDestoryEvent?.Invoke(this);

        base.DestoryObject();
    }

    #endregion

    #region 행동 

    public override bool Detect()
    {
        if (currentState == E_MonsterState.Idle)
            return UnityEngine.Random.value <= detectValue / 100f;
        else
            return false;
    }

    public override void DetectAction()
    {
        SetState(E_MonsterState.Notice);
    }

    public virtual void NoticeAction()
    {
        SetState(E_MonsterState.Battle);
    }

    public virtual void DamagedAction()
    {
        if (currentState == E_MonsterState.Battle)
        {

        }
        else
        {
            SetState(E_MonsterState.Battle);
            Managers.Area.BattleStart();
        }
    }
    #endregion

    #region Battle
    public void Damaged(Part part, float damage)
    {
        DamagedAction();
    }
    public void BrokenPart(Part part)
    {
        if (part.PartType == E_PartType.Chest || part.PartType == E_PartType.Head)
            SetState(E_MonsterState.Dead);
    }
    #endregion

    #endregion

    public bool HavePart(E_PartType partType)
    {
        for(int i = 0; i < parts.Count; i++)
        {
            if (parts[i].PartType == partType && parts[i].IsBroken == false)
                return true;
        }
        return false;
    }

    public Part GetPart(E_PartType partType)
    {
        Part part = null;
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].PartType == partType && parts[i].IsBroken == false)
                return parts[i];
        }
        return part;
    }

    #region UI
    protected CreaturePartUi partUi;

    public CreaturePartUi PartUi { get => partUi; }
    public void Open(bool isOn)
    {
        partUi.OpenClose(isOn);
    }
    #endregion
}
