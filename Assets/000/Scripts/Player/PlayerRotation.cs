using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerRotation : PlayerControllerComponent
    {
        [SerializeField] private Transform playerModel;
        [SerializeField] private float rotationSpeed;

        [SerializeField] private List<PlayerState> validStateList;

        private HashSet<PlayerState> validStates;

        private void Awake()
        {
            validStates = validStateList.ToHashSet();
        }

        private void Update()
        {
            if (playerInfo.lastMove.sqrMagnitude != 0 && validStates.Contains(playerInfo.stateManager.state))
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerInfo.lastMove, Vector3.up);
                playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
