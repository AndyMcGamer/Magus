using FishNet.Object;
using Magus.Game;
using Magus.Multiplayer;
using Magus.Skills;
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
    }

    public class PlayerSkillManager : PlayerControllerComponent
    {
        [Header("References")]
        [SerializeField] private PlayerAttack playerAttack;

        [Header("Skill Lists")]
        [SerializeField] private List<ActiveSkill> activeSkills;
        [SerializeField] private List<PassiveSkillData> passiveSkills;

        public event Action<int> OnSkillUpdate;

        private void Awake()
        {
            OnSkillUpdate?.Invoke(ConnectionManager.instance.playerData[base.LocalConnection]);
        }

        private void OnEnable()
        {
            GlobalPlayerController.instance.OnSkillUpdate += SkillUpdated;
        }

        private void OnDisable()
        {
            GlobalPlayerController.instance.OnSkillUpdate -= SkillUpdated;
        }

        private void SkillUpdated(int playerNumber, string skillName)
        {
            if (playerNumber != ConnectionManager.instance.playerData[base.LocalConnection]) return;
            OnSkillUpdate?.Invoke(playerNumber);
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
        }

        public void ActivateSkill(int skillNumber)
        {
            ActiveSkillData skillData = activeSkills[skillNumber - 1].skillData;
            if (skillData == null || activeSkills[skillNumber - 1].cooldown > 0) return;
            activeSkills[skillNumber - 1].cooldown = skillData.Cooldown[0]; // replace with proper level
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
            GlobalPlayerController.instance.UpdateSkillStatus(ConnectionManager.instance.playerData[base.LocalConnection], skillName, addingPoint);
        }
    }
}
