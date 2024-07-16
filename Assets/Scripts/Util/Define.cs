using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum E_PartType
    {
        Head,
        Chest,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg
    }

    public enum E_MonsterState
    {
        None,
        Idle,
        Battle,
        BattleWait,
        Run,
        SucessRun,
        Dead,
    }

    public enum E_ObjectType
    {
        None,
        Monster,
        Item,
    }

    public enum E_AreaState
    {
        None,
        Exploring,  //≈Ω«Ë¡ﬂ
        Explor_End,
        Explore_Stop,
        Battle_Start,
        Battle_End
    }
}
