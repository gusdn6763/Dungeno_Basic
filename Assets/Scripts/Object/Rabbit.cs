using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Rabbit : Creature
{
    public override void DamagedAction()
    {
        SetState(E_MonsterState.Run);
    }

    public override void NoticeAction()
    {
        SetState(E_MonsterState.Run);
    }
}
