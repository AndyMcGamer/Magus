using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Magus.Skills.SkillTree
{
    [System.Serializable]
    public class SkillPrerequisite
    {
        public SkillData skill;
        [Min(1)] public int requiredLevel = 1;
    }

    [CreateAssetMenu(fileName = "New SkillNode", menuName = "Magus/Skills/SkillNode")]
    public class SkillNode : ScriptableObject
    {
        public SkillData skillData;
        public List<SkillPrerequisite> prerequisites;
        [ReadOnly] public List<string> postrequisites;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(skillData != null && this.name != skillData.Name)
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), skillData.Name);
                AssetDatabase.SaveAssetIfDirty(this);
            }
        }
#endif
    }
}
