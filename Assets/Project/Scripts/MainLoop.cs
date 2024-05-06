using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BluMarble.Core
{
    public class MainLoop : BluMarble.Singleton.Singleton<MainLoop>
    {
        private Dictionary<GameState, Action> m_ActionList;
        private GameState m_CurrentGameState = GameState.None;

        public GameState CurrentGameState
        { 
            get 
            { 
                return m_CurrentGameState; 
            }
        }

        private void Awake()
        {
            // Load game
            m_CurrentGameState = GameState.Loading;
            BluMarble.Events.EventsManager.Instance.m_LoadingStarted.Invoke();
            StartLoading();
            BluMarble.Events.EventsManager.Instance.m_LoadingEnded.Invoke();

            // Cache game actions
            m_ActionList = new Dictionary<GameState, Action>();
            m_ActionList.Add(GameState.Loading, DoNothing);
            m_ActionList.Add(GameState.Start, StartGame);
            m_ActionList.Add(GameState.Running, UpdateGame);
            m_ActionList.Add(GameState.End, EndGame);
            m_ActionList.Add(GameState.Finished, FinishGame);
        }

        private void StartLoading()
        {
            // Validate all singletons
#if UNITY_EDITOR
            if (BluMarble.Procedural.ProceduralManager.Instance == null)
            {
                Assert.IsTrue(false, "ProceduralManager not found!");
                return;
            }
            if (BluMarble.UI.UIManager.Instance == null)
            {
                Assert.IsTrue(false, "UIManager not found!");
                return;
            }
            if (BluMarble.Events.EventsManager.Instance == null)
            {
                Assert.IsTrue(false, "EventsManager not found!");
                return;
            }
#endif
            // Init all singletons
            BluMarble.Procedural.ProceduralManager.Instance.PerformInit();
            BluMarble.UI.UIManager.Instance.PerformInit();
            BluMarble.Events.EventsManager.Instance.PerformInit();

            m_CurrentGameState = GameState.Start;
        }

        private void Update()
        {
            m_ActionList[m_CurrentGameState].Invoke();
        }

        private void StartGame()
        {
            BluMarble.Procedural.ProceduralManager.Instance.PerformStart();
            BluMarble.UI.UIManager.Instance.PerformStart();
            BluMarble.Events.EventsManager.Instance.PerformStart();

            m_CurrentGameState = GameState.Running;
        }

        private void UpdateGame()
        {
            BluMarble.Procedural.ProceduralManager.Instance.PerformUpdate();
            BluMarble.UI.UIManager.Instance.PerformUpdate();
        }

        private void EndGame()
        {
            BluMarble.Procedural.ProceduralManager.Instance.PerformEnd();
            BluMarble.UI.UIManager.Instance.PerformEnd();
        }

        private void FinishGame()
        {
            BluMarble.Procedural.ProceduralManager.Instance.PerformFinish();
            BluMarble.UI.UIManager.Instance.PerformFinish();

            // Cleanup here
        }

        private void DoNothing()
        {
        }
    }
}
