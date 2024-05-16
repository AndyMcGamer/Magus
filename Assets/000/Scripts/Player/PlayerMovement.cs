using FishNet.Object;
using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerMovement : PlayerControllerComponent
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform playerModel;

        [Header("Settings")]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float gravity;

        private Vector3 moveInput;
        private Vector3 Gravity => gravity * Vector3.down;

        public override void OnStartClient()
        {
            base.OnStartClient();
            playerInfo.lastMove = new Vector3(-1,0,1).normalized;
            if (!base.IsOwner)
            {
                characterController.enabled = false;
                enabled = false;
            }
        }

        public void OnMove(Vector2 input)
        {
            moveInput = new(input.x, 0, input.y);
            moveInput.ConvertToIsometric(playerInfo.playerCamera.transform.eulerAngles.ScaleBy(Vector3.up));
        }

        private void Update()
        {
            Vector3 movement = (moveInput * playerInfo.moveSpeed + Gravity) * Time.deltaTime;
            characterController.Move(movement);
            if (moveInput.sqrMagnitude != 0)
            {
                playerInfo.lastMove = moveInput;
            }

            if(playerInfo.lastMove.sqrMagnitude != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerInfo.lastMove, Vector3.up);
                playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
        }
    }
}
