using Cinemachine;
using FishNet.Object;
using Magus.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magus.PlayerController
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputProcessor : PlayerControllerComponent
    {
        private PlayerInput playerInput;

        public override void OnStartClient()
        {
            base.OnStartClient();
            playerInput = GetComponent<PlayerInput>();
            if (!base.IsOwner)
            {
                playerInput.DeactivateInput();
                enabled = false;
            }
            else
            {
                playerInput.enabled = true;
            }
        }
        public void OnMove(InputAction.CallbackContext value)
        {
            Vector2 input = value.ReadValue<Vector2>();
            playerInfo.movement.OnMove(input);
        }

        public void OnOpenSkill(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.playerHUD.ToggleSkillScreen();
            }
        }

        public void OnOpenStat(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.playerHUD.ToggleStatScreen();
            }
        }

        public void OnSkill_1(InputAction.CallbackContext value)
        {
            if(value.started)
            {
                playerInfo.skillManager.ActivateSkill(1);
            }
        }

        public void OnSkill_2(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.skillManager.ActivateSkill(2);
            }
        }
    }
}
