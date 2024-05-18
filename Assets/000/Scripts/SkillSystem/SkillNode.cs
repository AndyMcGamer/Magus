using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills
{
    [System.Serializable]
    public class SkillPrerequisite
    {
        public SkillData skill;
        [Min(1)] public int requiredLevel;
    }

    [System.Serializable]
    public class SkillNode
    {
        public SkillData skillData;
        public List<SkillPrerequisite> prerequisites;
    }
}
