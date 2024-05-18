using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills
{
    [CreateAssetMenu(fileName = "New SkillDatabase", menuName = "Magus/Skills/SkillDatabase")]
    public class SkillDatabase : ScriptableObject
    {
        public SkillNode[] skills;

        public T FindByName<T>(string name) where T : SkillData
        {
            var sd = System.Array.Find(skills, x => x.skillData.Name == name);
            return sd as T;
        }
    }
}
