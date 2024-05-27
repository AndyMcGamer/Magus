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
