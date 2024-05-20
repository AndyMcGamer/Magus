using FishNet.Object;
using Magus.Game;
using Magus.Global;
using Magus.Multiplayer;
using Magus.Skills;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    [System.Serializable]
    public class ActiveSkill
    {
        public ActiveSkillData skillData;
        [HideInInspector] public float cooldown;

        public ActiveSkill(ActiveSkillData sd)
        {
            skillData = sd;
            cooldown = 0;
        }
    }

    public class PlayerSkillManager : PlayerControllerComponent
    {
        [Header("References")]
        [SerializeField] private PlayerAttack playerAttack;

        [Header("Skill Lists")]
        [SerializeField, ReorderableList, ReadOnly] private List<ActiveSkill> activeSkills;
        [SerializeField, ReorderableList, ReadOnly] private List<PassiveSkillData> passiveSkills;

        private HashSet<SkillData> skillList;

        public event Action<int, string> OnSkillUpdate;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner) return;
            RebuildSkillLists();
            GlobalPlayerController.instance.OnSkillUpdate += SkillUpdated;
            GlobalPlayerController.instance.OnSkillAdded += OnSkillAdded;
            GlobalPlayerController.instance.OnSkillRemoved += OnSkillRemoved;
        }

        private void OnDestroy()
        {
            GlobalPlayerController.instance.OnSkillUpdate -= SkillUpdated;
            GlobalPlayerController.instance.OnSkillAdded -= OnSkillAdded;
            GlobalPlayerController.instance.OnSkillRemoved -= OnSkillRemoved;
        }

        private void SkillUpdated(int playerNumber, string skillName)
        {
            if (playerNumber != ConnectionManager.instance.playerData[base.LocalConnection]) return;
            OnSkillUpdate?.Invoke(playerNumber, skillName);
        }
        private void OnSkillAdded(int playerNumber, string skillName)
        {
            if (playerNumber != ConnectionManager.instance.playerData[base.LocalConnection]) return;
            SkillData sd = GlobalPlayerController.instance.skillDatabase.FindDataByName<SkillData>(skillName);
            skillList.Add(sd);
            AddSkillToList(sd);

            OnSkillUpdate?.Invoke(playerNumber, skillName);
        }

        private void OnSkillRemoved(int playerNumber, string skillName, int skillLevel)
        {
            if (playerNumber != ConnectionManager.instance.playerData[base.LocalConnection]) return;

            SkillData sd = GlobalPlayerController.instance.skillDatabase.FindDataByName<SkillData>(skillName);
            skillList.Remove(sd);
            RemoveSkillFromList(sd);

            // Refund points
            GlobalPlayerController.instance.ChangeSkillPoints(playerNumber, skillLevel);
            OnSkillUpdate?.Invoke(playerNumber, skillName);
        }

        private void Update()
        {
            foreach (ActiveSkill skill in activeSkills)
            {
                if(skill.cooldown > 0)
                {
                    skill.cooldown -= Time.deltaTime;
                }
            }

            foreach (PassiveSkillData passiveSkill in passiveSkills)
            {
                
            }
        }

        public void ActivateSkill(int skillNumber)
        {
            string skillName = GlobalPlayerController.instance.hotbarSkills[skillNumber - 1];
            ActiveSkillData skillData = GlobalPlayerController.instance.skillDatabase.FindDataByName<ActiveSkillData>(skillName);
            ActiveSkill activeSkill = activeSkills.Find(x => x.skillData == skillData);

            if (activeSkill == null) return;
            
            if (activeSkill.cooldown > 0) return;
            int skillLevel = GlobalPlayerController.instance.GetSkillStatus(ConnectionManager.instance.playerData[base.LocalConnection])[skillData.Name];
            activeSkill.cooldown = skillData.Cooldown[skillLevel - 1];
            switch (skillData.skillType)
            {
                case ActiveSkillType.Projectile:
                    playerAttack.CastProjectileSkill(skillData as ProjectileSkillData);
                    break;
                case ActiveSkillType.Movement:

                    break;
                case ActiveSkillType.Toggle:
                    break;
                case ActiveSkillType.Summon:
                    break;
                default:
                    break;
            }
        }

        public void UpdateSkill(string skillName, bool addingPoint)
        {
            GlobalPlayerController.instance.ChangeSkillPoints(ConnectionManager.instance.playerData[base.LocalConnection], addingPoint ? -1 : 1);
            GlobalPlayerController.instance.UpdateSkillStatus(ConnectionManager.instance.playerData[base.LocalConnection], skillName, addingPoint);
        }

        public void PropogateSkillUpdate(string skillName)
        {
            OnSkillUpdate?.Invoke(ConnectionManager.instance.playerData[base.LocalConnection], skillName);
        }

        private void RebuildSkillLists()
        {
            skillList = new();
            passiveSkills = new();
            activeSkills = new();
            foreach (var skillStatus in GlobalPlayerController.instance.GetSkillStatus(ConnectionManager.instance.playerData[base.LocalConnection]))
            {
                SkillData sd = GlobalPlayerController.instance.skillDatabase.FindDataByName<SkillData>(skillStatus.Key);
                if (sd is PassiveSkillData)
                {
                    passiveSkills.Add(sd as PassiveSkillData);
                }
                else if (sd is ActiveSkillData)
                {
                    activeSkills.Add(new ActiveSkill(sd as ActiveSkillData));
                }
            }
        }

        private void AddSkillToList(SkillData sd)
        {
            if(sd is PassiveSkillData)
            {
                passiveSkills.Add(sd as PassiveSkillData);
            }
            else if(sd is ActiveSkillData)
            {
                activeSkills.Add(new ActiveSkill(sd as ActiveSkillData));

                // Add to hotbar if space
                for (int i = 0; i < Constants.MAX_HOTBAR_SKILLS; i++)
                {
                    if (GlobalPlayerController.instance.hotbarSkills[i] == default)
                    {
                        GlobalPlayerController.instance.hotbarSkills[i] = sd.Name;
                        break;
                    }
                }
            }
        }

        private void RemoveSkillFromList(SkillData sd)
        {
            if(sd is PassiveSkillData)
            {
                passiveSkills.Remove(sd as PassiveSkillData);
            }
            else if (sd is ActiveSkillData)
            {
                ActiveSkill activeSkill = activeSkills.Find(x => x.skillData == sd);
                activeSkills.Remove(activeSkill);

                // Remove from hotbar
                for (int i = 0; i < Constants.MAX_HOTBAR_SKILLS; i++)
                {
                    if (GlobalPlayerController.instance.hotbarSkills[i] == sd.Name)
                    {
                        GlobalPlayerController.instance.hotbarSkills[i] = default;
                        break;
                    }
                }
            }
        }
    }
}
