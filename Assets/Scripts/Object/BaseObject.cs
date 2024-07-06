using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    protected bool init = false;

    protected virtual bool Init()
    {
        if (init)
            return false;

        init = true;
        return true;
    }

    private void Awake()
    {
        Init();
    }
}
