using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Delegate 
    public Action keyAction = null;


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
        if (keyAction != null)
            keyAction.Invoke();
    }

    [Header("����Ŭ�� ���ѽð�")]
    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;

    public bool CheckDoubleClick()
    {
        if (m_IsOneClick && ((Time.time - m_Timer) > m_DoubleClickSecond))
        {
            m_IsOneClick = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!m_IsOneClick)
            {
                m_Timer = Time.time;
                m_IsOneClick = true;
            }
            else if (m_IsOneClick && ((Time.time - m_Timer) < m_DoubleClickSecond))
            {
                m_IsOneClick = false;
                return true;
            }
        }
        return false;
    }
}
