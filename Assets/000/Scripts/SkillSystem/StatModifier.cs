using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills
{
    public enum ModifierType
    {
        Constant,
        Percentage
    }

    [System.Serializable]
    public class StatModifier
    {
        public StatType stat;
        public ModifierType type;
        public float value;
    }

    [System.Serializable]
    public class StatModifierList
    {
        public List<StatModifier> modifiers;
    }
}
