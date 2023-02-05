using System.Collections.Generic;
using UnityEngine;

namespace FBird
{
    [DisallowMultipleComponent]
    public class ObjectScroller : MonoBehaviour
    {
        [SerializeField] private float m_ScrollSpeed = 1.0f;
        [SerializeField] private float m_ResetAt = -20.0f;
        [SerializeField] private float m_Offset = 9.0f;

        [SerializeField] private bool m_UpdateIfPlayState = false;
        [SerializeField] private bool m_ApplyRandomYOffset = false;
        [SerializeField]private int m_MinYOffset = -2, m_MaxYOffset = 2;

        private LinkedList<Transform> m_ChildTransforms;

        private void Start()
        {
            var childCount = transform.childCount;
            m_ChildTransforms = new LinkedList<Transform>();
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                m_ChildTransforms.AddLast(child);
                if (m_ApplyRandomYOffset)
                    ApplyRandomYOffset(child);
            }
        }

        private void Update()
        {
            if ((m_UpdateIfPlayState && GameManager.CurrentState != GameState.Play) || GameManager.CurrentState == GameState.GameOver)
                return;

            float delta = Time.deltaTime;

            var first = m_ChildTransforms.First.Value;
            var pos = first.position;
            pos.x -= m_ScrollSpeed * delta;

            if(pos.x <= -m_ResetAt)
            {
                if(m_ApplyRandomYOffset)
                    ApplyRandomYOffset(first);
                m_ChildTransforms.RemoveFirst();
                m_ChildTransforms.AddLast(first);
                pos = m_ChildTransforms.First.Value.position;
            }

            int index = 0;
            for (LinkedListNode<Transform> node = m_ChildTransforms.First; node != null; node = node.Next)
            {
                var nPos = node.Value.position;
                nPos.x = pos.x + (index * m_Offset);
                node.Value.position = nPos;
                index++;
            }
        }

        private void ApplyRandomYOffset(Transform t)
        {
            var pos = t.position;
            pos.y = Random.Range(m_MinYOffset, m_MaxYOffset);
            t.position = pos;
        }
    }
}