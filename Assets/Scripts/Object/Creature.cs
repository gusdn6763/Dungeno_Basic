using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static UnityEngine.LightAnchor;

public class Creature : InteractionObject
{
    public Action<Creature> OnDestoryEvent;

    protected CreaturePartUi creaturePartUi;

    [Header("스텟")]
    [SerializeField] private Stat stat;

    [Header("체력")]
    [SerializeField] private float hp;
    public float Hp { get => hp; set => hp = value; }

    [Header("부위")]
    public List<CreaturePart> parts = new List<CreaturePart>();

    [Header("감지 확률")]
    public float detectValue;

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

        ObjectType = E_ObjectType.Monster;

        for(int i = 0; i < parts.Count; i++)
        {
            CreaturePart creaturePart = parts[i];
            creaturePart.Init(this);
            creaturePart.OnDamaged += Damaged;
            creaturePart.OnBroken += BrokenPart;
        }

        creaturePartUi = GetComponentInChildren<CreaturePartUi>();
        return true;
    }

    void OnBecameInvisible() //화면밖으로 나가 보이지 않게 되면 호출이 된다.
    {
        currentState = E_MonsterState.SucessRun;
    }

    public override void Spawn()
    {
        currentState = E_MonsterState.Idle;
        StartCoroutine(StateUpdate());
    }

    #region AI

    #region 실시간
    protected virtual IEnumerator StateUpdate()
    {
        while (true)
        {
            switch (currentState)
            {
                case E_MonsterState.Idle:
                    UpdateIdle();
                    break;
                case E_MonsterState.Battle:
                    UpdateBattle();
                    break;
                case E_MonsterState.BattleWait:
                    UpdateAttack();
                    break;
                case E_MonsterState.Run:
                    UpdateRun();
                    break;
                case E_MonsterState.Dead:
                    UpdateDead();
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected virtual void UpdateIdle()
    {
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

    protected virtual void UpdateAttack()
    {

    }
    protected virtual void UpdateRun()
    {
        transform.Translate(runDirection * speed * Time.deltaTime);
    }
    protected virtual void UpdateDead()
    {

    }
    #endregion

    #region 상태 최초 실행
    public void StateEnter(E_MonsterState monsterState)
    {
        if (currentState != monsterState)
        {
            switch (monsterState)
            {
                case E_MonsterState.Run:
                    EnterRun();
                    break;
                case E_MonsterState.Battle:
                    EnterBattle();
                    break;
                case E_MonsterState.BattleWait:
                    EnterWait();
                    break;
                case E_MonsterState.Dead:
                case E_MonsterState.SucessRun:
                    DestoryObject();
                    break;
            }
        }
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
        Managers.Game.GameStateEnter(E_GameState.Battle_Start);
    }

    public void EnterWait()
    {
        text.color = Color.red;
    }

    public override void DestoryObject()
    {
        base.DestoryObject();
        OnDestoryEvent?.Invoke(this);
        Managers.Object.Despawn(this);
    }

    #endregion

    #region 상태 
    [Header("확인용-현재상태")]
    [SerializeField] protected E_MonsterState currentState = E_MonsterState.None;

    public E_MonsterState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            StateEnter(value);
            currentState = value;
        }
    }

    public bool Detect()
    {
        if (currentState == E_MonsterState.Idle)
            return UnityEngine.Random.value <= detectValue / 100f;
        else
            return false;
    }

    public virtual void DetectAction()
    {
        CurrentState = E_MonsterState.Battle;
    }

    public virtual void DamagedAction()
    {
        CurrentState = E_MonsterState.Battle;
    }
    #endregion

    #region Battle
    public void Damaged(Part part, float damage)
    {
        DamagedAction();
        //플레이어 선공
        Managers.Battle.PlayerMonsterAttack(Index);
    }
    public void BrokenPart(Part part)
    {
        if (part.PartType == E_PartType.Chest || part.PartType == E_PartType.Head)
            CurrentState = E_MonsterState.Dead;
    }
    #endregion

    #endregion

    public bool HavePart(E_PartType partType)
    {
        for(int i = 0; i < parts.Count; i++)
        {
            if (parts[i].PartType == partType)
                return true;
        }
        return false;
    }

    public Part GetPart(E_PartType partType)
    {
        Part part = null;
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].PartType == partType)
                return parts[i];
        }
        return part;
    }
}
