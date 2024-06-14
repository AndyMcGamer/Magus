using FishNet.Object;
using Magus.Global;
using System;
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

        private string currentControlScheme;
        private string chosenActionMap;

        public event Action OnLoadedProcessor;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner)
            {
                playerInput.DeactivateInput();
                enabled = false;
            }
            else
            {
                playerInput.enabled = true;
                currentControlScheme = playerInput.currentControlScheme;
                chosenActionMap = playerInput.currentActionMap.name;
                playerInput.SwitchCurrentActionMap(chosenActionMap);
                OnLoadedProcessor?.Invoke();
            }
        }
        public void OnMove(InputAction.CallbackContext value)
        {
            Vector2 input = value.ReadValue<Vector2>();
            playerInfo.movement.OnMove(input);
        }

        public Vector2 GetMoveInput()
        {
            var action = playerInput.currentActionMap.FindAction("Move");
            if (action == null) return Vector2.zero;
            return action.ReadValue<Vector2>();
        }

        public string GetActionName(string actionName)
        {
            if (playerInput == null || playerInput.currentActionMap == null) return default;
            var action = playerInput.currentActionMap.FindAction(actionName);
            if (action == null) return default;
            return action.GetBindingDisplayString(InputBinding.MaskByGroup(currentControlScheme));
        }

        public void OnControlsChanged()
        {
            if (playerInput.currentControlScheme != currentControlScheme)
            {
                currentControlScheme = playerInput.currentControlScheme;
            }
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

        public void OnZoom(InputAction.CallbackContext value)
        {
            float scrollValue = value.ReadValue<float>();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            scrollValue /= 120f;
#endif

            playerInfo.playerCamControl.OnZoom(scrollValue);
        }

        public void OnOpenPause(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.playerHUD.TogglePauseScreen();
            }
        }

        public void TogglePlayerInput(bool enable)
        {
            if(enable)
            {
                playerInput.SwitchCurrentActionMap(chosenActionMap);
            }
            else
            {
                playerInput.SwitchCurrentActionMap(Constants.MENU_ACTION_MAP);
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

        public void OnSkill_3(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.skillManager.ActivateSkill(3);
            }
        }

        public void OnSkill_4(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.skillManager.ActivateSkill(4);
            }
        }
        public void OnSkill_5(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.skillManager.ActivateSkill(5);
            }
        }

        public void OnSkill_6(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.skillManager.ActivateSkill(6);
            }
        }

        public void OnSkill_7(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.skillManager.ActivateSkill(7);
            }
        }

        public void OnSkill_8(InputAction.CallbackContext value)
        {
            if (value.started)
            {
                playerInfo.skillManager.ActivateSkill(8);
            }
        }

    }
}
