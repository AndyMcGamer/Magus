using Magus.Global;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Skills
{
    public abstract class SkillData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; set; }
        [field: SerializeField, TextArea] public string Description { get; set; }
        [field: SerializeField, Min(1)] public int MaxLevel { get; set; }
        public virtual float[] Cost { get; protected set; }
        public virtual float[] Cooldown { get; protected set; }

        public virtual string GetDescription(int level) {
            return Description.Replace(Constants.SKILL_NAME, Name).Replace(Constants.SKILL_COOLDOWN, Cooldown[level].ToString()).Replace(Constants.SKILL_COST, Cost[level].ToString());
        }

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            Cost = HelperFunctions.ExtendArray(Cost, MaxLevel).ToArray();
            Cooldown = HelperFunctions.ExtendArray(Cooldown, MaxLevel).ToArray();
        }
#endif
    }
}
