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
        // �Է� ���� Ű�� �ƹ��͵� ���ٸ� ����
        if (Input.anyKey == false) 
            return;

        // � Ű�� ���Դٸ�, keyaction���� �̺�Ʈ�� �߻������� ����. 
        if (keyaction != null)
        {
            keyaction.Invoke();
        }
    }
}
