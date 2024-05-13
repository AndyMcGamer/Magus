using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Global
{
    public enum Comparator
    {
        Less,
        Greater,
        GreaterOrEqual,
        LessOrEqual,
        Equal,
        NotEqual,
    }

    public abstract class Conditional<T> : ScriptableObject
    {
        public abstract bool Evaluate(T input);
    }
}
