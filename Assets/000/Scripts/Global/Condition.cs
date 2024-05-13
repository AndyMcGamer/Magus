using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Global
{
    [System.Serializable]
    public abstract class Condition<T>
    {
        public List<Conditional<T>> conditionals;
        public virtual bool Evaluate(T input)
        {
            bool returnValue = false;
            foreach (var conditional in conditionals)
            {
                returnValue &= conditional.Evaluate(input);
            }
            return returnValue;
        }
    }
}
