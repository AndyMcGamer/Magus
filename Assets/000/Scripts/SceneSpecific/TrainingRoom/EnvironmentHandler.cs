using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class EnvironmentHandler : NetworkBehaviour
    {
        [SerializeField] public GameObject environment;
        public override void OnStartClient()
        {
            base.OnStartClient();
            environment.SetActive(true);
            print(environment.scene.handle);
            print(ServerManager.Clients[base.LocalConnection.ClientId].Scenes.First(x => x.name == "TrainingRoom").handle);
        }
    }
}
