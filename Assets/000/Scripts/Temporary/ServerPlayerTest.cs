using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Temporary
{
    public class ServerPlayerTest : NetworkBehaviour
    {
        private void Update()
        {
            print(transform.position);
        }
    }
}
