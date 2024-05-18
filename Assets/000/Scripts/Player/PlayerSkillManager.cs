using FishNet.Object;
using Magus.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerSkillManager : PlayerControllerComponent
    {
        [Header("References")]
        [SerializeField] private PlayerAttack playerAttack;

        [Header("Skill Lists")]
        [SerializeField] private ActiveSkillData[] activeSkills;
        [SerializeField] private List<PassiveSkillData> passiveSkills;

        private float[] activeCooldowns;

        private void Awake()
        {
            activeCooldowns = new float[activeSkills.Length];
        }

        private void Update()
        {
            for (int i = 0; i < activeCooldowns.Length; i++)
            {
                if (activeCooldowns[i] > 0)
                {
                    activeCooldowns[i] -= Time.deltaTime;
                }
            }
        }

        public void ActivateSkill(int skillNumber)
        {
            ActiveSkillData skillData = activeSkills[skillNumber - 1];
            if (skillData == null || activeCooldowns[skillNumber - 1] > 0) return;
            activeCooldowns[skillNumber - 1] = skillData.Cooldown[0]; // replace with proper level
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
    }
}
