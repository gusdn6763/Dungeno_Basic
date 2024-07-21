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
        Notice,
        Battle,
        BattleWait,
        Run,
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
        Exploring,      //Ž����
        Explore_Stop,   //Ž�����
        Explore_End,    //Ž��Ϸ�
        Battle_Start,
        Battle_End
    }
}
