using Magus.Global;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Skills
{
    [CreateAssetMenu(fileName = "New ProjectileSkill", menuName = "Magus/Skills/ProjectileSkillData")]
    public class ProjectileSkillData : ActiveSkillData
    {
        public GameObject projectilePrefab;
        public override ActiveSkillType SkillType { get => ActiveSkillType.Projectile; }
        [field: SerializeField] public override float[] Cost { get; protected set; }
        [field: SerializeField] public override float[] Cooldown { get; protected set; }

        public float[] damage;
        public float[] moveRate;
        public float[] lifetime;

        public Vector3 spawnOffset;

        public override string GetDescription(int level)
        {
            string description = base.GetDescription(level);
            return description.Replace(Constants.SKILL_DAMAGE, damage[level].ToString());
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            base.OnValidate();
            damage = HelperFunctions.ExtendArray(damage, MaxLevel).ToArray();
            moveRate = HelperFunctions.ExtendArray(moveRate, MaxLevel).ToArray();
            lifetime = HelperFunctions.ExtendArray(lifetime, MaxLevel).ToArray();
        }
#endif
    }
}
