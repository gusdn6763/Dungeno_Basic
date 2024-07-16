using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Rabbit : Creature
{
    public override IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionTime);
        SetState(E_MonsterState.Run);
    }

    public override void DamagedAction()
    {
        SetState(E_MonsterState.Run);
    }
}
