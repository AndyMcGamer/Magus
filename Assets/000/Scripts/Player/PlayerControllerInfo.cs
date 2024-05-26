using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using Magus.Game;
using Magus.Global;
using Magus.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerControllerInfo : NetworkBehaviour
    {
        [Header("References")]
        public InputProcessor inputProcessor;
        public PlayerStateManager stateManager;
        public PlayerRotation playerRotation;
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
            stateManager.Init(this);
            playerRotation.Init(this);
            movement.Init(this);
            playerHUD.Init(this);
            skillManager.Init(this);
            playerAttack.Init(this);

            GlobalPlayerController.instance.OnPlayerDeath += PlayerDeath;
        }

        private void OnDestroy()
        {
            GlobalPlayerController.instance.OnPlayerDeath -= PlayerDeath;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            playerTag = HelperFunctions.GetPlayerTag(ConnectionManager.instance.playerData[base.Owner]);
            gameObject.tag = playerTag;
            playerCollider.tag = playerTag;
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
            gameObject.tag = tag;
            playerCollider.tag = tag;
        }

        private void PlayerDeath(int playerNumber)
        {
            if (playerNumber == ConnectionManager.instance.playerData[base.Owner])
            {
                DespawnPlayer(gameObject);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnPlayer(GameObject playerObject)
        {
            ServerManager.Despawn(playerObject);
        }
    }
}
