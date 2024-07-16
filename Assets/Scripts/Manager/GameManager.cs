using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class GameManager : MonoBehaviour
{
    public Action<E_AreaState> onStateChange;

    [SerializeField] private Button startButton;
    [SerializeField] private CameraSetting cameraSetting;

    [Header("���� ���� ����")]
    [SerializeField] private E_AreaState currentGameState;
    public E_AreaState CurrentGameState { get => currentGameState; }

    [Header("Ž�� ī��Ʈ �ð�")]
    [SerializeField] private float advantureCountTime;

    [Header("Ȯ�ο�-����ð�")]
    [SerializeField] private float currentAdvantureTime = 0f;

    [Header("Ž�� ī��Ʈ �ð� ������")]
    [SerializeField] private int advantureCountAddTime;

    private Camera mainCamera;
    public Camera MainCamera { get => mainCamera; }
    public void Init()
    {
        mainCamera = Camera.main;
        cameraSetting.Init(mainCamera);
        startButton.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        GameStateEnter(E_AreaState.Explor_End);
    }

    #region Update ���� �ǽð� ����
    private void Update()
    {
        //����Ŭ�� �Ǵ� �����̽��ٽ� ���� ���¸� ����
        HandleInput();

        //���� ���º� ����
        GameStateUpdate();
    }

    public void GameStateUpdate()
    {
        switch (currentGameState)
        {
            case E_AreaState.Exploring:
                ProcessExplorationTime();
                break;
        }
    }
    [SerializeField] TextMeshProUGUI text;
    public void ProcessExplorationTime()
    {
        //����׿�
        {
            text.text = (advantureCountTime - currentAdvantureTime).ToString("F2");
        }

        currentAdvantureTime += Time.deltaTime;

        if (currentAdvantureTime >= advantureCountTime)
        {
            currentAdvantureTime = 0;
            Managers.Player.AddTime(advantureCountAddTime);
            GameStateEnter(E_AreaState.Explor_End);
        }
    }

    #endregion

    #region ���� ��ȭ�� 1ȸ ����
    public void GameStateEnter(E_AreaState gameState)
    {
        currentAdvantureTime = 0;
        if (gameState != currentGameState)
        {
            currentGameState = gameState;
            onStateChange?.Invoke(currentGameState);
        }
    }

    #endregion

    #region ����Ŭ��

    [Header("����Ŭ�� ���ѽð�")]
    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;
    private void HandleInput()
    {
        if (CheckDoubleClick() || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentGameState == E_AreaState.Exploring)
                GameStateEnter(E_AreaState.Explore_Stop);
            else if (currentGameState == E_AreaState.Explore_Stop)
                GameStateEnter(E_AreaState.Exploring);
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
