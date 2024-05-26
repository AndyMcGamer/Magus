using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public enum PlayerState
    {
        Idle = 0,
        Moving = 10,
        Casting = 50,
        Frozen = 99998,
        Dead = 99999
    }

    public class PlayerStateManager : PlayerControllerComponent
    {
        public event Action<PlayerState> OnEnterState;
        public event Action<PlayerState> OnExitState;

        [SerializeField] private PlayerState defaultState;

        [ReadOnly] public PlayerState state;
        [ReadOnly] public int priority;

        private void Awake()
        {
            state = defaultState;
            OnEnterState?.Invoke(defaultState);
        }

        public bool ChangeState(PlayerState newState, bool forceRefresh = false)
        {
            if ((int)newState < priority || ((newState == state) && !forceRefresh)) return false;

            OnExitState?.Invoke(state);
            state = newState;
            priority = (int)newState;
            OnEnterState?.Invoke(state);
            return true;
        }

        public bool ChangeState(PlayerState newState, int newPriority, bool forceRefresh = false)
        {
            if(newPriority <  priority || ((newState == state) && !forceRefresh)) return false;

            OnExitState?.Invoke(state);
            state = newState;
            priority = newPriority;
            OnEnterState?.Invoke(state);
            return true;
        }

        public void ExitState()
        {
            OnExitState?.Invoke(state);
            state = defaultState;
            priority = (int)defaultState;
            OnEnterState?.Invoke(state);
        }

        public void ExitState(PlayerState exitState)
        {
            if (state != exitState) return;
            ExitState();
        }
    }
}
