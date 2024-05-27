using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Skills
{
    public enum ActiveSkillType
    {
        Projectile,
        Dash,
        Toggle,
        Summon
    }
    public abstract class ActiveSkillData : SkillData
    {
        public abstract ActiveSkillType SkillType { get; }
        public int priority;
        public float[] spellTime;
        public float[] castTime;

#if UNITY_EDITOR
        public override void OnValidate()
        {
            base.OnValidate();
            castTime = HelperFunctions.ExtendArray(castTime, MaxLevel).ToArray();
            spellTime = HelperFunctions.ExtendArray(spellTime, MaxLevel).ToArray();
            for (int i = 0; i < MaxLevel; i++)
            {
                if (castTime[i] > spellTime[i])
                {
                    castTime[i] = spellTime[i];
                }
            }
        }
#endif
    }
}
