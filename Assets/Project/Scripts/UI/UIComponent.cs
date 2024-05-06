using JetBrains.Annotations;
using System;
using UnityEngine;

namespace BluMarble.UI
{
    [Serializable]
    public class UIComponent
    {
        [SerializeField]
        private GameObject m_UIVisual;
        public GameObject UIVisual
        {
            get { return m_UIVisual; }
        }

        [SerializeField]
        private UIState m_UIGameState;
        public UIState UIGameState
        {
            get
            {
                return m_UIGameState;
            }
        }
    }
}
