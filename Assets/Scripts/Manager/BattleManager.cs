using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BattleManager : MonoBehaviour
{
    public List<Creature> allCreature = new List<Creature>();
    public int FightMonsterLevel { get; set; }

    public void Spawn<T>(T obj) where T : Creature
    {
        allCreature.Add(obj);
    }

    public void Despawn<T>(T obj) where T : Creature
    {
        allCreature.Remove(obj);

        if (HaveStateMonster(E_MonsterState.Battle))
        {
            // 전투 중인 몬스터가 여전히 있음
        }
        else if (HaveStateMonster(E_MonsterState.BattleWait))
        {
            List<Creature> waitCreatures = GetStateMonster(E_MonsterState.BattleWait);
            foreach (var creature in waitCreatures)
            {
                creature.CurrentState = E_MonsterState.Battle;
            }
        }
        else
        {
            Managers.Game.GameStateEnter(E_GameState.Exploring);
        }
    }

    public List<Creature> GetStateMonster(E_MonsterState monsterState)
    {
        return allCreature.FindAll(creature => creature.CurrentState == monsterState);
    }

    public bool HaveStateMonster(E_MonsterState monsterState)
    {
        return allCreature.Exists(creature => creature.CurrentState == monsterState);
    }

    public void CheckMonsterDetection()
    {
        foreach (var creature in allCreature)
        {
            if (creature.Detect())
                creature.DetectAction();
        }
    }

    public void PlayerMonsterAttack(int creatureLevel)
    {
        FightMonsterLevel = creatureLevel;

        if (HaveStateMonster(E_MonsterState.Battle))
            BattleStart();
    }

    public void BattleStart()
    {
        SetHighLevelMonsterBehavior();
        SetMidLevelMonsterBehavior();
        SetLowLevelMonsterBehavior();
        Managers.Game.GameStateEnter(E_GameState.Battle_Start);
    }

    public void SetHighLevelMonsterBehavior()
    {
        foreach (var creature in allCreature)
        {
            if (creature.Index > FightMonsterLevel)
            {
                int value = Random.Range(0, 4);
                switch (value)
                {
                    case 0: // 즉시 참여
                        FightMonsterLevel = creature.Index;
                        break;
                    case 1: // 연속 참여
                        creature.CurrentState = E_MonsterState.BattleWait;
                        break;
                    case 2: // 무관심
                        break;
                    case 3: // 도망
                        creature.CurrentState = E_MonsterState.Run;
                        break;
                }
            }
        }
    }

    public void SetMidLevelMonsterBehavior()
    {
        foreach (var creature in allCreature)
        {
            if (creature.Index == FightMonsterLevel)
                creature.CurrentState = E_MonsterState.Battle;
        }
    }

    public void SetLowLevelMonsterBehavior()
    {
        foreach (var creature in allCreature)
        {
            if (creature.Index < FightMonsterLevel)
                creature.CurrentState = E_MonsterState.Run;
        }
    }
}