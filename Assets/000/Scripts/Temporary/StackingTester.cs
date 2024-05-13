using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Temporary
{
    public class StackingTester : NetworkBehaviour
    {
        public override void OnStartServer()
        {
            base.OnStartServer();
            print(UnityEngine.SceneManagement.SceneManager.sceneCount);
        }
    }
}
