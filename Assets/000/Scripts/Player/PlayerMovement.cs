using FishNet.Object;
using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerMovement : PlayerControllerComponent
    {        

        [Header("Settings")]
        [SerializeField] private float gravity;

        private Vector3 moveInput;
        private Vector3 Gravity => gravity * Vector3.down;

        private bool canMove;

        public override void OnStartClient()
        {
            base.OnStartClient();
            playerInfo.lastMove = new Vector3(-1,0,1).normalized;
            if (!base.IsOwner)
            {
                playerInfo.characterController.enabled = false;
                enabled = false;
            }
            else
            {
                enabled = true;
                playerInfo.stateManager.OnEnterState += OnStateEnter;
                playerInfo.stateManager.OnExitState += OnStateExit;
            }
        }

        private void OnDestroy()
        {
            playerInfo.stateManager.OnEnterState -= OnStateEnter;
            playerInfo.stateManager.OnExitState -= OnStateExit;
        }

        private void OnStateEnter(PlayerState state)
        {
            canMove = state == PlayerState.Moving;
            if (!canMove) OnMove(playerInfo.inputProcessor.GetMoveInput());
        }

        private void OnStateExit(PlayerState state)
        {
            canMove = state == PlayerState.Moving;
        }

        public void OnMove(Vector2 input)
        {
            moveInput = new(input.x, 0, input.y);
            moveInput.ConvertToIsometric(playerInfo.playerCamera.transform.eulerAngles.ScaleBy(Vector3.up));

            playerInfo.playerAnim.targetSpeed = moveInput.sqrMagnitude;

            if(moveInput.sqrMagnitude > 0)
            {
                playerInfo.stateManager.ChangeState(PlayerState.Moving);
                if(canMove) playerInfo.lastMove = moveInput;
            }
            else if (canMove)
            {
                playerInfo.stateManager.ExitState();
            }
        }

        private void Update()
        {
            if (!canMove) return;

            Vector3 movement = (moveInput * playerInfo.moveSpeed + Gravity) * Time.deltaTime;
            playerInfo.characterController.Move(movement);
            
        }
    }
}
