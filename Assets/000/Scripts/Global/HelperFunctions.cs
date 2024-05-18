using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Global
{
    public static class HelperFunctions
    {
        public static bool ConditionalCompare<T>(T a, T b, Comparator op) where T : IComparable<T>
        {
            return op switch
            {
                Comparator.Less => a.CompareTo(b) < 0,
                Comparator.Greater => a.CompareTo(b) > 0,
                Comparator.GreaterOrEqual => a.CompareTo(b) >= 0,
                Comparator.LessOrEqual => a.CompareTo(b) <= 0,
                Comparator.Equal => a.CompareTo(b) == 0,
                Comparator.NotEqual => a.CompareTo(b) != 0,
                _ => false
            };
        }

        public static IList<T> ExtendArray<T>(IList<T> source, int size)
        {
            if(source.Count == size) return source;
            if(source.Count > size) return source.Take(size).ToArray();

            T[] temp = new T[size];

            Array.Copy(source.ToArray(), temp, source.Count);
            return temp;
        }

        public static Vector3 ToIsometric(this Vector3 source, Vector3 eulerAngles)
        {
            Quaternion rotation = Quaternion.Euler(eulerAngles);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
            return rotationMatrix.MultiplyPoint3x4(source);
        }

        public static Vector3 ToIsometric(this Vector3 source, Quaternion rotation)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
            return rotationMatrix.MultiplyPoint3x4(source);
        }

        public static void ConvertToIsometric(ref this Vector3 source, Vector3 eulerAngles)
        {
            Quaternion rotation = Quaternion.Euler(eulerAngles);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
            source = rotationMatrix.MultiplyPoint3x4(source);
        }

        public static void ConvertToIsometric(ref this Vector3 source, Quaternion rotation)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
            source = rotationMatrix.MultiplyPoint3x4(source);
        }

        public static Vector3 ScaleBy(this Vector3 v, Vector3 other) => new Vector3(other.x * v.x, other.y * v.y, other.z * v.z);

        public static string GetPlayerTag(int playerNumber) => playerNumber == 1 ? Constants.PLAYER_ONE_TAG : Constants.PLAYER_TWO_TAG;
    }
}
