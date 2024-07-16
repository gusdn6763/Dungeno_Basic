using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Delegate 
    public Action keyaction = null;



    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        // 입력 받은 키가 아무것도 없다면 종료
        if (Input.anyKey == false) 
            return;

        // 어떤 키가 들어왔다면, keyaction에서 이벤트가 발생했음을 전파. 
        if (keyaction != null)
        {
            keyaction.Invoke();
        }
    }
}
