using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BluMarble.UI
{
    public class UIManager : BluMarble.Singleton.Singleton<UIManager>
    {
        [SerializeField]
        private List<UIComponent> m_UIComponents;

        private UIState m_CurrentUIState = UIState.None;
        private UIState m_PreviousUIState = UIState.None;
        private Dictionary<UIState, UIComponent> m_UIStateComponentMap;

        public void Init()
        {
            m_CurrentUIState = UIState.None;
            m_PreviousUIState = UIState.None;
            m_UIStateComponentMap = new Dictionary<UIState, UIComponent>();

            SetUIStateComponentMap();

            HideAllUIScreens();

            SetCurrentState(UIState.MainMenu);

            // Events
            //GameManager.Instance.m_LoadingStarted.AddListener(OnLoadingStarted);
            //GameManager.Instance.m_LoadingEnded.AddListener(OnLoadingEnded);
            //GameManager.Instance.m_GameFinished.AddListener(OnGameFinished);
        }

        public void GoBackToPreviousScreen()
        {
            SetCurrentState(m_PreviousUIState);
        }

        public void SetCurrentState(UIState UINewState)
        {
            if (UINewState == m_CurrentUIState)
            {
                return;
            }

            if (m_CurrentUIState != UIState.None)
            {
                HideCurrentUIScreen();
            }

            m_PreviousUIState = m_CurrentUIState;
            m_CurrentUIState = UINewState;

            if (UINewState != UIState.None)
            {
                ShowCurrentUIScreen();
            }
        }

        private void ShowCurrentUIScreen()
        {
            m_UIStateComponentMap[m_CurrentUIState].UIVisual.SetActive(true);
        }

        private void HideCurrentUIScreen()
        {
            m_UIStateComponentMap[m_CurrentUIState].UIVisual.SetActive(false);
        }

        private void HideAllUIScreens()
        {
            foreach (var Item in m_UIStateComponentMap)
            {
                Item.Value.UIVisual.SetActive(false);
            }
        }

        private void SetUIStateComponentMap()
        {
            foreach (var UIComp in m_UIComponents)
            {
                m_UIStateComponentMap.Add(UIComp.UIGameState, UIComp);
            }
        }

    }
}
