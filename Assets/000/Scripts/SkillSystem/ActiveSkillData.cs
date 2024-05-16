using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills
{
    public enum ActiveSkillType
    {
        Projectile,
        Movement,
        Toggle,
        Summon
    }
    public abstract class ActiveSkillData : SkillData
    {
        public ActiveSkillType skillType;
    }
}
