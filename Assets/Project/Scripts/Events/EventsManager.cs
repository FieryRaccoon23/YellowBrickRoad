using UnityEngine;
using UnityEngine.Events;

namespace BluMarble.Events
{
    public class EventsManager : BluMarble.Singleton.Singleton<EventsManager>
    {
        public UnityEvent m_LoadingStarted;
        public UnityEvent m_LoadingEnded;
    }
}
