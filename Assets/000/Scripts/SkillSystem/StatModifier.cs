using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Magus.Global;

namespace Magus.Skills
{

    [System.Serializable]
    public class StatModifier
    {
        public StatType stat;
        public StatValueType type;
        public float value;
    }

    [System.Serializable]
    public class StatModifierList
    {
        public List<StatModifier> modifiers;
    }
}
