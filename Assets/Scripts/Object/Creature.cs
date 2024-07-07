using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Creature : InteractionObject
{
    public Action<int> OnDamagedEvent;

    [Header("부위 UI바")]
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
        creaturePartUi.Init();
        return true;
    }

    void OnBecameInvisible() //화면밖으로 나가 보이지 않게 되면 호출이 된다.
    {
        Destroy(gameObject); //객체를 삭제한다.
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
    protected virtual void UpdateBattle()
    {
    }
    protected virtual void UpdateAttack()
    {

    }
    protected virtual void UpdateRun()
    {
        transform.Translate(new Vector3(runPositionX * speed, runPositionY * speed));
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
                    EnterDead();
                    break;
            }
        }
    }

    private float runPositionX;
    private float runPositionY;
    public void EnterRun()
    {
        text.color = Color.blue;
        runPositionX = UnityEngine.Random.Range(-1f, 1f);
        runPositionY = UnityEngine.Random.Range(-1f, 1f);
    }

    public void EnterBattle()
    {
        text.color = Color.red;
    }

    public void EnterWait()
    {
        text.color = Color.red;
    }

    public void EnterDead()
    {
        Destroy(gameObject);
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
    protected virtual void OnDead()
    {
        CurrentState = E_MonsterState.Dead;
    }
    public void Damaged(Part part, float damage)
    {
        DamagedAction();
        //플레이어 선공
        OnDamagedEvent?.Invoke(Index);
    }
    public void BrokenPart(Part part)
    {
        if (part.PartType == E_PartType.Chest || part.PartType == E_PartType.Head)
            OnDead();
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
