using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Global;
using Magus.Multiplayer;
using Magus.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magus.Game
{
    public class GlobalPlayerController : NetworkBehaviour
    {
        public static GlobalPlayerController instance;

        public SkillDatabase skillDatabase;

        private readonly SyncVar<float> health_PlayerOne = new(new SyncTypeSettings(1f));
        private readonly SyncVar<float> health_PlayerTwo = new(new SyncTypeSettings(1f));

        public event Action<int> OnPlayerDeath;
        public event Action<int, float> OnPlayerHealthChange;

        public Dictionary<int, Scene> trainingRooms;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            trainingRooms = new();

            health_PlayerOne.OnChange += Health_PlayerOne_OnChange;
            health_PlayerTwo.OnChange += Health_PlayerTwo_OnChange;
        }

        
        private void Health_PlayerOne_OnChange(float prev, float next, bool asServer)
        {
            if (!asServer)
            {
                OnPlayerHealthChange?.Invoke(1, next);
            }
            else
            {
                if(next <= 0)
                {
                    OnPlayerDeath?.Invoke(1);
                }
            }
        }

        private void Health_PlayerTwo_OnChange(float prev, float next, bool asServer)
        {
            if (!asServer)
            {
                OnPlayerHealthChange?.Invoke(2, next);
            }
            else
            {
                if (next <= 0)
                {
                    OnPlayerDeath?.Invoke(2);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerHealth(NetworkConnection conn, float health)
        {
            int playerNumber = ConnectionManager.instance.playerData[conn];
            if (playerNumber == 1)
            {
                health_PlayerOne.Value = health;
            }
            else if (playerNumber == 2)
            {
                health_PlayerTwo.Value = health;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerHealth(int playerNumber, float health)
        {
            if (playerNumber == 1)
            {
                health_PlayerOne.Value = health;
            }
            else if (playerNumber == 2)
            {
                health_PlayerTwo.Value = health;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangePlayerHealth(NetworkConnection conn, float health)
        {
            int playerNumber = ConnectionManager.instance.playerData[conn];
            if(playerNumber == 1)
            {
                health_PlayerOne.Value += health;
            }
            else if(playerNumber == 2)
            {
                health_PlayerTwo.Value += health;
            }
            print($"{health_PlayerOne.Value} {health_PlayerTwo.Value}");
        }

        public Vector3 GetSpawnpoint(NetworkConnection conn)
        {
            return ConnectionManager.instance.playerData[conn] == 1 ? Constants.PLAYER_ONE_SPAWN : Constants.PLAYER_TWO_SPAWN;
        }

        [Server]
        public void AddTrainingRoom(int playerNumber, Scene scene)
        {
            trainingRooms.Add(playerNumber, scene);
        }

        [Server]
        public void ClearTrainingRooms()
        {
            trainingRooms.Clear();
        }
    }
}
