using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using Magus.Global;
using Magus.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerControllerInfo : NetworkBehaviour
    {
        [Header("References")]
        public InputProcessor inputProcessor;
        public PlayerMovement movement;
        public PlayerHUD playerHUD;
        public PlayerSkillManager skillManager;
        public PlayerAttack playerAttack;
        public CinemachineVirtualCamera playerCamera;
        public Collider playerCollider;

        [Header("Settings")]
        public string playerTag;
        public float moveSpeed = 5f;
        public Vector3 lastMove;

        private void Awake()
        {
            inputProcessor.Init(this);
            movement.Init(this);
            playerHUD.Init(this);
            skillManager.Init(this);
            playerAttack.Init(this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            gameObject.tag = ConnectionManager.instance.playerData[base.Owner] == 1 ? Constants.PLAYER_ONE_TAG : Constants.PLAYER_TWO_TAG;
            playerCollider.enabled = false;
            if (!base.IsOwner)
            {
                enabled = false;
            }
            else
            {
                playerCamera.Priority = 10;
                SetTag(gameObject.tag);
            }
        }

        [ServerRpc]
        private void SetTag(string tag)
        {
            playerCollider.tag = tag;
        }
    }
}
