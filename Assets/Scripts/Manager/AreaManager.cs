using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class AreaManager : MonoBehaviour
{
    [SerializeField] private Area currentArea;

    [Header("현재 지역 상태")]
    [SerializeField] private E_AreaState areaState;
    public E_AreaState AreaState { get => areaState; }

    [Header("탐험 카운트 시간")]
    [SerializeField] private float advantureCountTime;
    private float currentAdvantureTime = 0f;

    [Header("탐험 카운트 시간 증가량")]
    [SerializeField] private int advantureCountAddTime;

    public void Init()
    {
        Managers.Input.keyAction -= OnKeyboard;
        Managers.Input.keyAction += OnKeyboard;

        if (currentArea)
            currentArea.Init();
    }
    public void GameStart()
    {
        AreaStateEnter(E_AreaState.Exploring);
    }
    void OnKeyboard()
    {
        if (Input.GetKey(KeyCode.Space) || Managers.Input.CheckDoubleClick())
        {
            if (areaState == E_AreaState.Exploring)
                AreaStateEnter(E_AreaState.Explore_Stop);
            else if (areaState == E_AreaState.Explore_Stop)
                AreaStateEnter(E_AreaState.Exploring);
        }
    }

    #region 지역 상태 변경
    private void Update()
    {
        AreaStateChange();

        //게임 상태별 진행
        AreaStateUpdate();
    }
    public void AreaStateChange()
    {
        if (HaveStateCreature(E_MonsterState.Battle))
        {
            AreaStateEnter(E_AreaState.Battle_Start);
        }
        else if (HaveStateCreature(E_MonsterState.BattleWait))
        {
            List<Creature> waitCreatures = GetStateCreature(E_MonsterState.BattleWait);
            foreach (var creature in waitCreatures)
                creature.SetState(E_MonsterState.Battle);
        }
        else
        {
            if (areaState == E_AreaState.Battle_Start)
                AreaStateEnter(E_AreaState.Battle_End);
            else if (areaState == E_AreaState.Battle_End)
                AreaStateEnter(E_AreaState.Explore_Stop);
            else if (areaState == E_AreaState.Explore_End)
                AreaStateEnter(E_AreaState.Exploring);
        }
    }

    public void AreaStateUpdate()
    {
        switch (areaState)
        {
            case E_AreaState.Exploring:
                ProcessExplorationTime();
                break;
        }
    }
    public void AreaStateEnter(E_AreaState state)
    {
        if (areaState != state)
        {
            areaState = state;

            currentArea.AreaStateEnter(areaState);
        }
    }

    [SerializeField] TextMeshProUGUI text;
    public void ProcessExplorationTime()
    {
        //디버그용
        {
            text.text = (advantureCountTime - currentAdvantureTime).ToString("F2");
        }

        currentAdvantureTime += Time.deltaTime;

        if (currentAdvantureTime >= advantureCountTime)
        {
            currentAdvantureTime = 0;
            Managers.Player.AddTime(advantureCountAddTime);

            AreaStateEnter(E_AreaState.Explore_End);
        }
    }
    #endregion

    #region 현재 지역에서 특정 오브젝트 찾기
    public List<Creature> GetStateCreature(E_MonsterState monsterState)
    {
        if (currentArea)
        {
            List<Creature> creatures = currentArea.FindTypeObjects<Creature>();

            return creatures.FindAll(creature => creature.CurrentState == monsterState);
        }
        else
            return null;
    }
    public bool HaveStateCreature(E_MonsterState monsterState)
    {
        List<Creature> creatures = currentArea.FindTypeObjects<Creature>();

        return creatures.Exists(creature => creature.CurrentState == monsterState);
    }
    #endregion

    public string GetAreaName()
    {
        return currentArea.AreaName;
    }

    public void BattleStart()
    {
        if (currentArea is DungeonArea)
            (currentArea as DungeonArea).BattleStart();
    }
}
