using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class Item : InteractionObject
{

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = E_ObjectType.Item;
        return true;
    }
}
