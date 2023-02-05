using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FBird
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float m_Gravity = -5.0f;
        [SerializeField] private float m_JumpForce = 2.0f;
        [SerializeField] private float m_RotationModifier = 2.0f;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private AudioSource m_Audio;
        [SerializeField] private AudioClip m_FlapAudio;
        [SerializeField] private AudioClip m_PointAudio;
        [SerializeField] private AudioClip m_HitAudio;
        [SerializeField] private AudioClip m_GameOverAudio;

        [SerializeField] private float m_MinBounds, m_MaxBounds;

        private InputManager m_Input;
        private Transform m_Transform;
        private Vector3 m_Velocity;
        private float m_Rotation;
        private bool m_CanControl = false;
        private bool m_IsPlaying = false;

        private void Start()
        {
            m_Input = InputManager.Instance;
            m_Transform = GetComponent<Transform>();
        }

        private void Update()
        {
            if (!m_IsPlaying) return;

            float delta = Time.deltaTime;

            m_Velocity.y += m_Gravity * delta;
            m_Rotation = m_Velocity.y * m_RotationModifier;

            if (m_Input.IsFlapInputDown && m_CanControl)
            {
                m_Velocity.y = m_JumpForce;
                m_Rotation = m_JumpForce;
                PlayAudio(m_FlapAudio);
            }

            m_Transform.position += m_Velocity * delta;
            m_Transform.rotation = Quaternion.Euler(0, 0, m_Rotation);

            BoundsCheck();
        }

        private void PlayAudio(AudioClip clip)
        {
            if (m_Audio == null || clip == null)
                return;

            m_Audio.PlayOneShot(clip);
        }

        private void BoundsCheck()
        {
            var pos = m_Transform.position;
            if (pos.y >= m_MaxBounds)
            {
                pos.y = m_MaxBounds;
            }
            else if (pos.y <= m_MinBounds)
            {
                pos.y = m_MinBounds;
                m_Velocity.y = 0;
                GameOver();
            }
            m_Transform.position = pos;
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
            switch (newState)
            {
                case GameState.None:
                case GameState.StartScreen:
                    m_CanControl = false;
                    m_IsPlaying = false;
                    break;
                case GameState.GameOver:
                    m_CanControl = false;
                    if(m_Animator != null)
                    {
                        m_Animator.Play("Bird_GameOver");
                    }
                    break;
                case GameState.Play:
                    m_CanControl = true;
                    m_IsPlaying = true;
                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameOver();
        }

        private void GameOver()
        {
            if (GameManager.CurrentState != GameState.GameOver)
            {
                PlayAudio(m_HitAudio);
                PlayAudio(m_GameOverAudio);
            }

            GameManager.Instance.SetState(GameState.GameOver);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ScoreManager.AddScore();
            PlayAudio(m_PointAudio);
        }

#if UNITY_EDITOR
        private BoxCollider2D m_DBox2d;

        private void OnDrawGizmosSelected()
        {
            Vector3 size = Vector3.one;
            if (m_DBox2d == null)
                m_DBox2d = GetComponent<BoxCollider2D>();
            else
                size = m_DBox2d.size;

            Gizmos.color = Color.red;
            // Top
            Vector3 start = new Vector3(-10, m_MaxBounds, 0);
            Vector3 end = new Vector3(10, m_MaxBounds, 0);
            Vector3 center = new Vector3(0, m_MaxBounds, 0);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireCube(center, size);

            // Bottom
            start = new Vector3(-10, m_MinBounds, 0);
            end = new Vector3(10, m_MinBounds, 0);
            center = new Vector3(0, m_MinBounds, 0);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}