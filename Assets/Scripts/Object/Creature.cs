using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : InteractionObject
{
    public Action<int> OnDamagedEvent;

    [Header("���� UI��")]
    protected CreaturePartUi creaturePartUi;

    [Header("����")]
    [SerializeField] private Stat stat;

    [Header("ü��")]
    [SerializeField] private float hp;
    public float Hp { get => hp; set => hp = value; }

    [Header("����")]
    public List<CreaturePart> parts = new List<CreaturePart>();

    [Header("���� Ȯ��")]
    public float detectValue;

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

    public override void Spawn()
    {
        currentState = E_MonsterState.Idle;
        StartCoroutine(StateUpdate());
    }

    #region AI

    [Header("Ȯ�ο�-�������")]
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
        return UnityEngine.Random.value <= detectValue / 100f;
    }

    public virtual void DetectAction()
    {
        CurrentState = E_MonsterState.Battle;
    }

    public virtual void DamagedAction()
    {
        CurrentState = E_MonsterState.Battle;
    }

    #region �ǽð�
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

    }
    protected virtual void UpdateDead()
    {

    }
    #endregion

    #region ���� ���� ����
    public void StateEnter(E_MonsterState monsterState)
    {
        if(currentState != monsterState)
        {
            switch (currentState)
            {
                case E_MonsterState.Battle:
                    EnterBattle();
                    break;
                case E_MonsterState.Run:
                    EnterRun();
                    break;
            }
        }
    }

    public void EnterBattle()
    {

    }

    private float runPositionX;
    private float runPositionY;
    public void EnterRun()
    {
        runPositionX = UnityEngine.Random.Range(-1, 1);
        runPositionY = UnityEngine.Random.Range(-1, 1);
    }
    #endregion


    public void Damaged(Part part, float damage)
    {
        if(true)
        {
            //�÷��̾� ����
            OnDamagedEvent?.Invoke(Index);
        }
        else
        {

        }
    }
    public void BrokenPart(Part part)
    {

    }
    #endregion
}
