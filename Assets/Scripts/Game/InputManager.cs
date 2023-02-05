using UnityEngine;
using UnityEngine.InputSystem;

namespace FBird
{
    [DisallowMultipleComponent]
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        public bool IsFlapInputDown { get; private set; }

        private PlayerControls m_PlayerControls;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            m_PlayerControls = new PlayerControls();
        }

        private void Start()
        {
            m_PlayerControls.Player.Flap.started += ctx =>
            {
                IsFlapInputDown = true;
            };
        }

        private void LateUpdate()
        {
            IsFlapInputDown = false;
        }

        private void OnEnable()
        {
            m_PlayerControls?.Enable();
        }

        private void OnDisable()
        {
            m_PlayerControls?.Disable();
        }
    }
}