using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills
{
    [CreateAssetMenu(fileName = "New StatConditional", menuName = "Magus/Conditionals/StatConditional")]
    public class StatConditional : Conditional<PlayerStats>
    {
        public StatType statType;
        public Comparator comparator;
        public float value;

        public override bool Evaluate(PlayerStats input)
        {
            return statType switch
            {
                StatType.Health => HelperFunctions.ConditionalCompare(value, input.Health, comparator),
                StatType.Attack => HelperFunctions.ConditionalCompare(value, input.Attack, comparator),
                StatType.Defense => HelperFunctions.ConditionalCompare(value, input.Defense, comparator),
                StatType.MoveSpeed => HelperFunctions.ConditionalCompare(value, input.MoveSpeed, comparator),
                _ => false
            };
        }
    }

    //public class TimeConditional : Conditional<PlayerStats>
    //{
    //    public float timer;
    //    public override bool Evaluate(PlayerStats input)
    //    {
    //        if (timer > 0) return true;
    //        timer -= Time.deltaTime;
    //        return false;
    //    }
    //}
}
