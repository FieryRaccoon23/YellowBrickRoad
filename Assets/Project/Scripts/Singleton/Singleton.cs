using UnityEngine;

namespace BluMarble.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindFirstObjectByType(typeof(T));
                }
                return m_Instance;
            }
        }

        void Awake()
        {
            if (m_Instance != null && m_Instance != this as T)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_Instance = this as T;
            }
        }
    }
}
