using FishNet.Object;
using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform vCamTransform;
        [SerializeField] private Transform playerModel;

        [Header("Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float gravity;

        private Vector3 moveInput;
        private Vector3 lastMove;
        private Vector3 Gravity => gravity * Vector3.down;

        public void OnMove(Vector2 input)
        {
            moveInput = new(input.x, 0, input.y);
            moveInput.ConvertToIsometric(vCamTransform.eulerAngles.ScaleBy(Vector3.up));
        }

        private void Update()
        {
            Vector3 movement = (moveInput * moveSpeed + Gravity) * Time.deltaTime;
            characterController.Move(movement);
            if (moveInput.sqrMagnitude != 0)
            {
                lastMove = moveInput;
            }

            if(lastMove.sqrMagnitude != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lastMove, Vector3.up);
                playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
        }
    }
}
