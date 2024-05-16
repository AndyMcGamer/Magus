using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Skills
{
    [CreateAssetMenu(fileName = "New PassiveSkill", menuName = "Magus/Skills/PassiveSkillData")]
    public class PassiveSkillData : SkillData
    {
        public override float[] Cost { get; protected set; } = { 0 };
        public override float[] Cooldown { get; protected set; } = { 0 };

        public StatModifierList[] statModifiers;

        public PassiveCondition conditions;
#if UNITY_EDITOR
        public override void OnValidate()
        {
            base.OnValidate();
            statModifiers = HelperFunctions.ExtendArray(statModifiers, MaxLevel).ToArray();
        }
#endif
    }
}
