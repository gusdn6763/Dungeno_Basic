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
        // 입력 받은 키가 아무것도 없다면 종료
        if (Input.anyKey == false) 
            return;

        // 어떤 키가 들어왔다면, keyaction에서 이벤트가 발생했음을 전파. 
        if (keyAction != null)
            keyAction.Invoke();
    }

    [Header("더블클릭 제한시간")]
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
