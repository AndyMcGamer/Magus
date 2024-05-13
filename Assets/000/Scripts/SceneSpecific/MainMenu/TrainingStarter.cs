using FishNet;
using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Managing.Scened;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Yak;
using Magus.Global;
using Magus.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class TrainingStarter : MonoBehaviour
    {
        public void StartTrainingRoom()
        {
            Multipass mp = InstanceFinder.TransportManager.GetTransport<Multipass>();
            mp.SetClientTransport<Yak>();

            mp.StartConnection(true, Constants.YAK_TRANSPORT_INDEX);
            ClientManager clientManager = InstanceFinder.ClientManager;
            clientManager.StartConnection();

            LoadParams loadParams = new LoadParams() { ServerParams = new object[] { Constants.MIN_PLAYERS_SOLO } };
            SceneSwitcher.instance.LoadGlobalNetworkedScene("LoadingScene", loadParams);
        }
    }
}
