using UnityEngine;

namespace BluMarble.Singleton
{
    public class GameSettings : Singleton<GameSettings>
    {
        [Header("Procedural Settings")]
        [Tooltip("Max speed at which the procedural object will be moved.")]
        [SerializeField]
        private float m_MaxProceduralSpeed = 10.0f;
        public float MaxProceduralSpeed
        {
            get { return m_MaxProceduralSpeed;}
        }

        [Tooltip("Min speed at which the procedural object will be moved.")]
        [SerializeField]
        private float m_MinProceduralSpeed = 1.0f;
        public float MinProceduralSpeed
        {
            get { return m_MinProceduralSpeed; }
        }
    }
}
