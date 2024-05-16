using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills
{
    [CreateAssetMenu(fileName = "New SkillDatabase", menuName = "Magus/Skills/SkillDatabase")]
    public class SkillDatabase : ScriptableObject
    {
        public SkillData skills;
    }
}
