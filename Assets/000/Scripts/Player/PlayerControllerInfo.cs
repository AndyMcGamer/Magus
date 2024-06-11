using Cinemachine;
using FishNet.Component.Animating;
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
        public PlayerAnimation playerAnim;
        public PlayerRotation playerRotation;
        public PlayerMovement movement;
        public PlayerHUD playerHUD;
        public PlayerHotbar playerHotbar;
        public PlayerSkillManager skillManager;
        public PlayerAttack playerAttack;
        public PlayerDash playerDash;
        public PlayerCameraController playerCamControl;
        public PlayerModelController modelController;
        public CinemachineVirtualCamera playerCamera;
        public CharacterController characterController;
        public Collider playerCollider;
        public Transform playerModel;
        public NetworkAnimator playerAnimator;

        [Header("Settings")]
        public string playerTag;
        public float moveSpeed = 5f;
        public Vector3 lastMove;

        private WaitForSeconds fiveSeconds = new (5f);

        private bool roundOver;

        private void Awake()
        {
            inputProcessor.Init(this);
            stateManager.Init(this);
            playerAnim.Init(this);
            playerRotation.Init(this);
            movement.Init(this);
            playerHUD.Init(this);
            playerHotbar.Init(this);
            skillManager.Init(this);
            playerAttack.Init(this);
            playerDash.Init(this);
            playerCamControl.Init(this);
            modelController.Init(this);
            roundOver = false;
        }

        private void OnDestroy()
        {
            GlobalPlayerController.instance.OnPlayerDeath -= PlayerDeath;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GlobalPlayerController.instance.OnPlayerDeath += PlayerDeath;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            int playerNumber = ConnectionManager.instance.playerData[base.Owner];
            playerTag = HelperFunctions.GetPlayerTag(playerNumber);
            gameObject.tag = playerTag;
            playerCollider.tag = playerTag;
            gameObject.layer = HelperFunctions.GetPlayerLayer(playerNumber);

            if (!base.IsOwner)
            {
                enabled = false;
            }
            else
            {
                SetTag(gameObject.tag);
            }
        }

        [ServerRpc]
        private void SetTag(string tag)
        {
            gameObject.tag = tag;
            playerCollider.tag = tag;
        }

        [Server]
        private void PlayerDeath(int playerNumber)
        {
            if (playerNumber != ConnectionManager.instance.playerData[base.Owner])
            {
                return;
            }
            if (roundOver) return;
            roundOver = true;
            StartCoroutine(EndRound());
            Die(playerNumber);
        }

        private IEnumerator EndRound()
        {
            RoundController.instance.SetChangeTimer(false);
            yield return fiveSeconds;
            RoundController.instance.EndRound();
        }

        [ObserversRpc]
        private void Die(int playerNumber)
        {
            playerCollider.enabled = false;
            characterController.enabled = false;
            if (playerNumber != ConnectionManager.instance.playerData[base.Owner])
            {
                return;
            }
            stateManager.ChangeState(PlayerState.Dead);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnPlayer(GameObject playerObject)
        {
            ServerManager.Despawn(playerObject);
        }
    }
}
