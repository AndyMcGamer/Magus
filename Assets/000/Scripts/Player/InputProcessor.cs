using Cinemachine;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magus.PlayerController
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputProcessor : NetworkBehaviour
    {
        private PlayerInput playerInput;

        [Header("References")]
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerHUD playerHUD;
        [SerializeField] private CinemachineVirtualCamera playercamera;

        public override void OnStartServer()
        {
            base.OnStartServer();
            playerInput = GetComponent<PlayerInput>();
            if (!base.IsHostStarted)
            {
                playerInput.DeactivateInput();
                movement.enabled = false;
                playerHUD.enabled = false;
                enabled = false;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            playerInput = GetComponent<PlayerInput>();
            if (!base.IsOwner)
            {
                playerInput.DeactivateInput();
                movement.enabled = false;
                playerHUD.enabled = false;
                enabled = false;
            }
            else
            {
                playercamera.Priority = 10;
            }
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            Vector2 input = value.ReadValue<Vector2>();
            movement.OnMove(input);
        }

        public void OnOpenSkill(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerHUD.ToggleSkillScreen();
            }
        }

        public void OnOpenStat(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerHUD.ToggleStatScreen();
            }
        }
    }
}
