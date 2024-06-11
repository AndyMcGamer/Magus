using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerAnimation : PlayerControllerComponent
    {
        [SerializeField] private float movementBlendSpeed;
        private float currentSpeed;
        [HideInInspector] public float targetSpeed;

        private PlayerState previousState;

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
            AnimatorStateInfo stateInfo = playerInfo.playerAnimator.Animator.GetCurrentAnimatorStateInfo(0);
            float currentLength = stateInfo.length;
            switch (state)
            {
                case PlayerState.Idle:
                case PlayerState.Moving:
                    playerInfo.playerAnimator.CrossFadeInFixedTime("Movement", previousState == PlayerState.Idle ? 0.05f * currentLength : 0.15f * currentLength, 0);
                    break;
                case PlayerState.Casting:
                    playerInfo.playerAnimator.CrossFadeInFixedTime("Attack01", 0.05f * currentLength, 0);
                    break;
                case PlayerState.Frozen:
                    break;
                case PlayerState.Dead:
                    playerInfo.playerAnimator.CrossFadeInFixedTime("Die", 0.1f * currentLength, 0);
                    break;
                default:
                    break;
            }
        }

        private void OnStateExit(PlayerState state)
        {
            previousState = state;
        }

        private void Update()
        {
            if (currentSpeed == targetSpeed) return;
            if(targetSpeed > currentSpeed)
            {
                currentSpeed += movementBlendSpeed;
                currentSpeed = Mathf.Clamp(currentSpeed, currentSpeed, targetSpeed);
            }
            else
            {
                currentSpeed -= movementBlendSpeed;
                currentSpeed = Mathf.Clamp(currentSpeed, targetSpeed, currentSpeed);
            }
            playerInfo.playerAnimator.Animator.SetFloat("Speed", currentSpeed);
        }
    }
}
