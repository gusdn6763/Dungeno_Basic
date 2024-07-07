using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Area currentArea;
    public Area CurrentArea { get => currentArea; set { currentArea = value; } }

    [Header("���� ���� ����")]
    [SerializeField] private E_GameState currentGameState;
    public E_GameState CurrentGameState { get => currentGameState; }

    [Header("Ž�� ī��Ʈ �ð�")]
    [SerializeField] private float advantureCountTime;

    [Header("Ȯ�ο�-����ð�")]
    [SerializeField] private float currentAdvantureTime = 0f;

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
            case E_GameState.Exploring:
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
            GameStateEnter(E_GameState.Explor_End);
        }
    }

    #endregion

    #region ���� ��ȭ�� 1ȸ ����
    public void GameStateEnter(E_GameState gameState)
    {
        if (gameState != currentGameState)
        {
            currentGameState = gameState;
            currentArea.StateEnter();
        }
    }
    #endregion

    public void Init()
    {
        startButton.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        GameStateEnter(E_GameState.Explor_End);
    }

    #region ����Ŭ��

    [Header("����Ŭ�� ���ѽð�")]
    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;
    private void HandleInput()
    {
        if (CheckDoubleClick() || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentGameState == E_GameState.Exploring)
                GameStateEnter(E_GameState.Explore_Stop);
            else if (currentGameState == E_GameState.Explore_Stop)
                GameStateEnter(E_GameState.Exploring);
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
