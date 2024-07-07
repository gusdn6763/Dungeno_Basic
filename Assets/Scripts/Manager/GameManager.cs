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

    [Header("현재 게임 상태")]
    [SerializeField] private E_GameState currentGameState;
    public E_GameState CurrentGameState { get => currentGameState; }

    [Header("탐험 카운트 시간")]
    [SerializeField] private float advantureCountTime;

    [Header("확인용-현재시간")]
    [SerializeField] private float currentAdvantureTime = 0f;

    #region Update 상태 실시간 실행
    private void Update()
    {
        //더블클릭 또는 스페이스바시 시작 상태를 변경
        HandleInput();

        //게임 상태별 진행
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
        //디버그용
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

    #region 상태 변화시 1회 실행
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

    #region 더블클릭

    [Header("더블클릭 제한시간")]
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
