using DG.Tweening;
using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Skills
{
    [CreateAssetMenu(fileName = "New DashSkill", menuName = "Magus/Skills/DashSkillData")]
    public class DashSkillData : ActiveSkillData
    {
        public override ActiveSkillType SkillType => ActiveSkillType.Dash;
        [field: SerializeField] public override float[] Cost { get; protected set; }
        [field: SerializeField] public override float[] Cooldown { get; protected set; }

        public Vector3 dashDirection;
        public Ease easeFunction = Ease.Linear;
        public float[] damage;
        public float[] dashDistance;
        public float[] dashDuration;

#if UNITY_EDITOR
        public override void OnValidate()
        {
            base.OnValidate();
            damage = HelperFunctions.ExtendArray(damage, MaxLevel).ToArray();
            dashDistance = HelperFunctions.ExtendArray(dashDistance, MaxLevel).ToArray();
            dashDuration = HelperFunctions.ExtendArray(dashDuration, MaxLevel).ToArray();
            for (int i = 0; i < MaxLevel; i++)
            {
                if (dashDuration[i] > spellTime[i] - castTime[i])
                {
                    dashDuration[i] = spellTime[i] - castTime[i];
                }
            }
        }
#endif
    }
}
