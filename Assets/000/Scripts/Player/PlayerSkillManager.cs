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

        public void ActivateSkill(int skillNumber)
        {
            ActivateActive(activeSkills[skillNumber - 1]);
        }

        private void ActivateActive(ActiveSkillData skillData)
        {
            if (skillData == null) return;
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
