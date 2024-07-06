using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Rabbit : Creature
{
    public override void DetectAction()
    {
        CurrentState = E_MonsterState.Run;
    }

    public override void DamagedAction()
    {
        CurrentState = E_MonsterState.Run;
    }
}
