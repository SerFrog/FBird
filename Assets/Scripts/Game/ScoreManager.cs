using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FBird
{
    [DisallowMultipleComponent]
    public class ScoreManager : MonoBehaviour
    {
        private const string HIGH_SCORE = "HighScore";

        private static ScoreManager m_Instance;

        [SerializeField] private TMP_Text m_ScoreText;
        [SerializeField] private TMP_Text m_HighScoreText;

        private int m_Score;
        private int m_HighScore;

        private void Awake()
        {
            if(m_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            m_Instance = this;
        }

        private void Start()
        {
            m_HighScore = PlayerPrefs.GetInt(HIGH_SCORE, 0);
            m_HighScoreText.text = $"High Score\n{m_HighScore}";
            UpdateUI();
        }

        public static void AddScore()
        {
            m_Instance.m_Score++;
            m_Instance.UpdateUI();
        }

        private void UpdateUI()
        {
            if (m_ScoreText == null)
                return;

            m_ScoreText.text = m_Score.ToString();
        }

        private void OnEnable()
        {
            GameManager.OnStateChange += OnStateChange;
        }

        private void OnDisable()
        {
            GameManager.OnStateChange -= OnStateChange;
        }

        private void OnDestroy()
        {
            GameManager.OnStateChange -= OnStateChange;
        }

        private void OnStateChange(GameState newState)
        {
            if (newState != GameState.GameOver)
                return;

            if(m_Score > m_HighScore)
            {
                m_HighScore = m_Score;
                PlayerPrefs.SetInt(HIGH_SCORE, m_HighScore);
            }

            m_HighScoreText.text = $"High Score\n{m_HighScore}";
        }
    }
}