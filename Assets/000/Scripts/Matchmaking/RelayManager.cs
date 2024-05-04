using FishNet;
using FishNet.Transporting.UTP;
using Magus.Global;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Magus.MatchmakingSystem
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager instance;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        public async Task<string> CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(Constants.MAX_PLAYERS - 1);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                var unityTransport = InstanceFinder.TransportManager.GetTransport<FishyUnityTransport>();
                unityTransport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

                if (InstanceFinder.ServerManager.StartConnection())
                {
                    LobbyManager.instance.UpdateGameStatus(true);
                    InstanceFinder.ClientManager.StartConnection();
                    LobbyManager.instance.SetAllocationId(allocation.AllocationId.ToString());
                    return joinCode;
                }
                return null;
            }
            catch (RelayServiceException)
            {
                return null;
            }
        }

        public async Task JoinRelay(string relayCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

                var unityTransport = InstanceFinder.TransportManager.GetTransport<FishyUnityTransport>();
                unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                LobbyManager.instance.UpdateGameStatus(true);
                LobbyManager.instance.SetAllocationId(joinAllocation.AllocationId.ToString());
                InstanceFinder.ClientManager.StartConnection();
            }
            catch (RelayServiceException)
            {

            }
        }
    }
}
