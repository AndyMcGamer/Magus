using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Global;
using Magus.Multiplayer;
using Magus.Skills;
using Magus.Skills.SkillTree;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

namespace Magus.Game
{
    public class GlobalPlayerController : NetworkBehaviour
    {
        public static GlobalPlayerController instance;

        public SkillDatabase skillDatabase;

        private readonly SyncVar<float> health_PlayerOne = new(new SyncTypeSettings(0f));
        private readonly SyncVar<float> health_PlayerTwo = new(new SyncTypeSettings(0f));

        public float GetCurrentHealth(int playerNumber) => playerNumber == 1 ? health_PlayerOne.Value : health_PlayerTwo.Value;

        public event Action<int> OnPlayerDeath;
        public event Action<int, float> OnPlayerHealthChange;

        private readonly SyncDictionary<string, int> skillStatus_PlayerOne = new(new SyncTypeSettings(0f));
        private readonly SyncDictionary<string, int> skillStatus_PlayerTwo = new(new SyncTypeSettings(0f));

        public Dictionary<string, int> GetSkillStatus(int playerNumber) => (playerNumber == 1) ? skillStatus_PlayerOne.Collection : skillStatus_PlayerTwo.Collection;

        public Dictionary<string, int> confirmedSkills;

        /// <summary>
        /// Update Skill: playerNumber, skillName
        /// </summary>
        public event Action<int, string> OnSkillUpdate;

        /// <summary>
        /// Skill Added: playerNumber, skillName
        /// </summary>
        public event Action<int, string> OnSkillAdded;

        /// <summary>
        /// Skill Removed: playerNumber, skillName, skillLevel
        /// </summary>
        public event Action<int, string, int> OnSkillRemoved;

        private readonly SyncVar<int> skillPoints_PlayerOne = new();
        private readonly SyncVar<int> skillPoints_PlayerTwo = new();

        public int GetSkillPoints(int playerNumber) => playerNumber == 1 ? skillPoints_PlayerOne.Value : skillPoints_PlayerTwo.Value;

        public event Action<int, int, int> OnSkillPointsChanged;

        public Dictionary<int, Scene> trainingRooms;

        [ReadOnly] public string[] hotbarSkills;
        public event Action<int> OnHotbarUpdated;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            skillDatabase.Initialize();

            trainingRooms = new();

            hotbarSkills = new string[Constants.MAX_HOTBAR_SKILLS];

            confirmedSkills = new();

            health_PlayerOne.OnChange += Health_PlayerOne_OnChange;
            health_PlayerTwo.OnChange += Health_PlayerTwo_OnChange;

            skillStatus_PlayerOne.OnChange += SkillStatus_PlayerOne_OnChange;
            skillStatus_PlayerTwo.OnChange += SkillStatus_PlayerTwo_OnChange;

            skillPoints_PlayerOne.OnChange += SkillPoints_PlayerOne_OnChange;
            skillPoints_PlayerTwo.OnChange += SkillPoints_PlayerTwo_OnChange;
        }

        private void OnDestroy()
        {
            health_PlayerOne.OnChange -= Health_PlayerOne_OnChange;
            health_PlayerTwo.OnChange -= Health_PlayerTwo_OnChange;

            skillStatus_PlayerOne.OnChange -= SkillStatus_PlayerOne_OnChange;
            skillStatus_PlayerTwo.OnChange -= SkillStatus_PlayerTwo_OnChange;

            skillPoints_PlayerOne.OnChange -= SkillPoints_PlayerOne_OnChange;
            skillPoints_PlayerTwo.OnChange -= SkillPoints_PlayerTwo_OnChange;
        }

        private void SkillStatus_PlayerOne_OnChange(SyncDictionaryOperation op, string key, int value, bool asServer)
        {
            if (asServer) return;
            switch (op)
            {
                case SyncDictionaryOperation.Add:
                    OnSkillAdded?.Invoke(1, key);
                    break;
                case SyncDictionaryOperation.Clear:
                    break;
                case SyncDictionaryOperation.Remove:
                    OnSkillRemoved?.Invoke(1, key, value);
                    break;
                case SyncDictionaryOperation.Set:
                    OnSkillUpdate?.Invoke(1, key);
                    break;
                case SyncDictionaryOperation.Complete:
                    break;
            }
        }

        private void SkillStatus_PlayerTwo_OnChange(SyncDictionaryOperation op, string key, int value, bool asServer)
        {
            if (asServer) return;
            switch (op)
            {
                case SyncDictionaryOperation.Add:
                    OnSkillAdded?.Invoke(2, key);
                    break;
                case SyncDictionaryOperation.Clear:
                    break;
                case SyncDictionaryOperation.Remove:
                    OnSkillRemoved?.Invoke(2, key, value);
                    break;
                case SyncDictionaryOperation.Set:
                    OnSkillUpdate?.Invoke(2, key);
                    break;
                case SyncDictionaryOperation.Complete:
                    break;
            }
        }

        private void SkillPoints_PlayerOne_OnChange(int prev, int next, bool asServer)
        {
            if (asServer) return;
            OnSkillPointsChanged?.Invoke(1, prev, next);
        }
        private void SkillPoints_PlayerTwo_OnChange(int prev, int next, bool asServer)
        {
            if (asServer) return;
            OnSkillPointsChanged?.Invoke(2, prev, next);
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

        [ObserversRpc(ExcludeServer = false)]
        public void LockSkills()
        {
            int playerNumber = ConnectionManager.instance.playerData[base.LocalConnection];
            confirmedSkills = new(GetSkillStatus(playerNumber));
        }

        public void RefundSkills()
        {
            confirmedSkills = new();
        }

        public void UpdateHotbar(int hotbarIndex)
        {
            OnHotbarUpdated?.Invoke(hotbarIndex);
        }

        public void SwapSkills(int index1, int index2)
        {
            (hotbarSkills[index1], hotbarSkills[index2]) = (hotbarSkills[index2], hotbarSkills[index1]);
            OnHotbarUpdated?.Invoke(index1);
            OnHotbarUpdated?.Invoke(index2);
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateSkillStatus(int playerNumber, string skillName, bool addingPoint)
        {
            int pointChange = addingPoint ? 1 : -1;
            if (playerNumber == 1)
            {
                if (skillStatus_PlayerOne.ContainsKey(skillName))
                {
                    skillStatus_PlayerOne[skillName] += pointChange;
                    if (skillStatus_PlayerOne[skillName] <= 0)
                    {
                        skillStatus_PlayerOne.Remove(skillName);
                    }
                }
                else if (addingPoint)
                {
                    skillStatus_PlayerOne.Add(skillName, 1);
                }
            }
            else if (playerNumber == 2)
            {
                if (skillStatus_PlayerTwo.ContainsKey(skillName))
                {
                    skillStatus_PlayerTwo[skillName] += pointChange;
                    if (skillStatus_PlayerTwo[skillName] <= 0) skillStatus_PlayerTwo.Remove(skillName);
                }
                else if (addingPoint)
                {
                    skillStatus_PlayerTwo.Add(skillName, 1);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveSkill(int playerNumber, string skillName)
        {
            if (playerNumber == 1 && skillStatus_PlayerOne.ContainsKey(skillName))
            {
                OnSkillRemoved?.Invoke(1, skillName, skillStatus_PlayerOne[skillName]);
                skillStatus_PlayerOne.Remove(skillName);
            }
            else if (playerNumber == 2 && skillStatus_PlayerTwo.ContainsKey(skillName))
            {
                OnSkillRemoved?.Invoke(2, skillName, skillStatus_PlayerTwo[skillName]);
                skillStatus_PlayerTwo.Remove(skillName);
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

        [Server]
        public void SetSkillPoints(int playerNumber, int amount)
        {
            if(playerNumber == 1)
            {
                skillPoints_PlayerOne.Value = amount;
            }
            else if(playerNumber == 2)
            {
                skillPoints_PlayerTwo.Value = amount;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeSkillPoints(int playerNumber, int amount)
        {
            if (playerNumber == 1)
            {
                skillPoints_PlayerOne.Value += amount;
            }
            else if (playerNumber == 2)
            {
                skillPoints_PlayerTwo.Value += amount;
            }
        }
    }
}
