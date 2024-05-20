using UnityEngine;
using UnityEngine.Events;

namespace BluMarble.Events
{
    public class EventsManager : BluMarble.Singleton.Singleton<EventsManager>
    {
        // Loading
        public UnityEvent m_LoadingStarted;
        public UnityEvent m_LoadingEnded;


        public override void PerformFinish()
        {
            m_LoadingStarted.RemoveAllListeners();
            m_LoadingEnded.RemoveAllListeners();
        }
    }
}
