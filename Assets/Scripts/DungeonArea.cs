using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonArea : MonoBehaviour
{
    public bool isStart = false;
    public MobSpawner spawner;

    void Update()
    {
        //더블클릭 또는 스페이스바시 시작 상태를 변경
        HandleInput();

        if (isStart)
        {
            if (spawner.CanSpawn(Time.deltaTime))
            {
                MobCommand mobCommand = spawner.SpawnMobCommand();
                if (mobCommand)
                {
                    Vector3 spawnPosition;
                    if (FindSpawnPosition(mobCommand, out spawnPosition))
                    {
                        mobCommand.transform.SetParent(transform);
                        mobCommand.transform.position = spawnPosition;
                    }
                    else
                    {
                        // 생성 포기
                        Destroy(mobCommand.gameObject);
                    }
                }
            }
        }
    }

    public bool FindSpawnPosition(MobCommand mobCommand, out Vector3 spawnPosition)
    {
        Vector2 size = mobCommand.GetSize();

        for (int i = 0; i < 1000; i++)
        {
            spawnPosition = GetRandomPositionInCameraView(size);
            if (Physics2D.OverlapBox(spawnPosition, size, 0) == false)
            {
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    public virtual Vector3 GetRandomPositionInCameraView(Vector3 size)
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        float randomX = Random.Range(bottomLeft.x + size.x, topRight.x - size.x);
        float randomY = Random.Range(bottomLeft.y + size.y, topRight.y - size.y);
        float z = transform.position.z;
        return new Vector3(randomX, randomY, z);
    }

    #region 더블클릭
    [Header("더블클릭 제한시간")]
    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;
    private void HandleInput()
    {
        if (CheckDoubleClick() || Input.GetKeyDown(KeyCode.Space))
        {
            isStart = !isStart;
            Debug.Log("상태 : " + isStart);
        }
    }

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
    #endregion
}
