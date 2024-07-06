using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum E_MonsterState
    {
        None,
        Idle,
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

    public enum E_GameState
    {
        None,
        Exploring,
        Explor_End,
        Explore_Stop,
        Battle_Start,
        Battle_End
    }
}
