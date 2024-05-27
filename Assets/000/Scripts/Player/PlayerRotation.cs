using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerRotation : PlayerControllerComponent
    {
        [SerializeField] private float rotationSpeed;

        private void Update()
        {
            if (playerInfo.lastMove.sqrMagnitude != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerInfo.lastMove, Vector3.up);
                playerInfo.playerModel.rotation = Quaternion.RotateTowards(playerInfo.playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
