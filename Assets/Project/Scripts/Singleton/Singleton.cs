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

        private void Awake()
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

        // Called only once
        public virtual void PerformInit()
        {

        }

        // Called every time the game starts e.g. entering from main menu to game
        public virtual void PerformStart()
        {

        }

        // Called on every update
        public virtual void PerformUpdate()
        {

        }

        // Called every time the game ends e.g. going from game to main menu
        public virtual void PerformEnd() 
        {

        }

        // Called once when the game is closed
        public virtual void PerformFinish()
        {

        }
    }
}
