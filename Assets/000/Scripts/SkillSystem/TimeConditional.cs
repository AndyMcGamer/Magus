using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Skills
{
    public class TimeConditional : Conditional<PlayerStats>
    {
        public float timeInterval;
        public override bool Evaluate(PlayerStats input)
        {
            timeInterval -= Time.deltaTime;
            return timeInterval > 0;
        }
    }
}
