using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class EnvironmentHandler : NetworkBehaviour
    {
        [SerializeField] public GameObject environment;
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (base.LocalConnection.Scenes.Contains(environment.scene))
            {
                environment.SetActive(true);
            }
        }
    }
}
