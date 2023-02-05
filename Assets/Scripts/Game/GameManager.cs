using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FBird
{
    public enum GameState
    {
        None = 0,
        StartScreen,
        Play,
        GameOver
    }

    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        public static Action<GameState> OnStateChange;
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameState m_StartState = GameState.StartScreen;
        [SerializeField] private GameObject m_StartScreen;
        [SerializeField] private GameObject m_GameOverScreen;
        [SerializeField] private GameObject m_ScoreObject;
        [SerializeField] private GameObject m_HighScoreObject;

        public static GameState CurrentState => Instance.m_CurrentState;

        private GameState m_CurrentState = GameState.None;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            SetState(m_StartState);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            switch (m_CurrentState)
            {
                case GameState.None:
                    SetState(GameState.StartScreen);
                    break;
                case GameState.StartScreen:
                    if (InputManager.Instance.IsFlapInputDown)
                        StartGame();
                    break;
                case GameState.Play:
                    break;
                case GameState.GameOver:
                    if (InputManager.Instance.IsFlapInputDown)
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
            }
        }

        public void SetState(GameState newState)
        {
            if (m_CurrentState == newState)
                return;

            HideAllScreens();

            switch (newState)
            {
                case GameState.None:
                    newState = GameState.StartScreen;
                    break;
                case GameState.StartScreen:
                    ShowHighScore();
                    ShowStartScreen();
                    break;
                case GameState.Play:
                    ShowScore();
                    break;
                case GameState.GameOver:
                    ShowGameOverScreen();
                    ShowHighScore();
                    ShowScore();
                    break;
            }

            m_CurrentState = newState;
            OnStateChange?.Invoke(m_CurrentState);
        }

        private void ShowStartScreen()
        {
            m_StartScreen?.SetActive(true);
        }

        private void ShowScore()
        {
            m_ScoreObject?.SetActive(true);
        }

        private void ShowHighScore()
        {
            m_HighScoreObject?.SetActive(true);
        }

        private void ShowGameOverScreen()
        {
            m_GameOverScreen?.SetActive(true);
        }

        private void HideAllScreens()
        {
            m_StartScreen?.SetActive(false);
            m_ScoreObject?.SetActive(false);
            m_GameOverScreen?.SetActive(false);
            m_HighScoreObject?.SetActive(false);
        }

        private void StartGame()
        {
            SetState(GameState.Play);
            m_StartScreen?.SetActive(false);
        }
    }
}