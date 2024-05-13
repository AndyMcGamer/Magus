using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Skills
{
    public abstract class SkillData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField, TextArea] public string Description { get; set; }
        [field: SerializeField] public int MaxLevel { get; set; }
        public virtual float[] Cost { get; protected set; }
        public virtual float[] Cooldown { get; protected set; }

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            Cost = HelperFunctions.ExtendArray(Cost, MaxLevel).ToArray();
            Cooldown = HelperFunctions.ExtendArray(Cooldown, MaxLevel).ToArray();
        }
#endif
    }
}
