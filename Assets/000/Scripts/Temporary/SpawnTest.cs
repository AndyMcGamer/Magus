using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Temporary
{
    public class SpawnTest : MonoBehaviour
    {
        public GameObject player;

        private void Awake()
        {
            Instantiate(player, Vector3.one * 2f, Quaternion.identity);
        }
    }
}
