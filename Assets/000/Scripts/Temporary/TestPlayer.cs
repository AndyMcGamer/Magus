using Cinemachine;
using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace Magus.Temporary
{
    public class TestPlayer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;
        private CharacterController controller;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private Vector3 moveInput;

        public void OnMove(InputAction.CallbackContext value)
        {
            Vector2 input = value.ReadValue<Vector2>();
            moveInput = new(input.x, 0, input.y);
            moveInput.ConvertToIsometric(vCam.transform.eulerAngles.ScaleBy(Vector3.up));
        }

        private void Update()
        {

            Vector3 movement = (moveInput * 5 + Vector3.down * 9.81f) * Time.deltaTime;
            controller.Move(movement);

        }
    }
}
