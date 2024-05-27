using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerAnimation : PlayerControllerComponent
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner) return;
            enabled = true;
            playerInfo.stateManager.OnEnterState += OnStateEnter;
            playerInfo.stateManager.OnExitState += OnStateExit;
        }

        private void OnDestroy()
        {
            playerInfo.stateManager.OnEnterState -= OnStateEnter;
            playerInfo.stateManager.OnExitState -= OnStateExit;
        }

        private void OnStateEnter(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    playerInfo.playerAnimator.Play("Idle03");
                    break;
                case PlayerState.Moving:
                    playerInfo.playerAnimator.Play("BattleRunForward");
                    break;
                case PlayerState.Casting:
                    playerInfo.playerAnimator.Play("Attack01");
                    break;
                case PlayerState.Frozen:
                    break;
                case PlayerState.Dead:
                    break;
                default:
                    break;
            }
        }

        private void OnStateExit(PlayerState state)
        {
            
        }
    }
}
