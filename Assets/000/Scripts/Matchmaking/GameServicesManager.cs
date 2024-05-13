using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;

namespace Magus.MatchmakingSystem
{
    public class GameServicesManager : MonoBehaviour
    {
        public static GameServicesManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeServices();
        }

        /// <summary>
        /// Initialize Unity Services and Sign In <br />
        /// Currently using Anonymous Sign In
        /// </summary>
        private async void InitializeServices()
        {
            await UnityServices.InitializeAsync();

            if(!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            (LobbyService.Instance as ILobbyServiceSDKConfiguration).EnableLocalPlayerLobbyEvents(true);
        }
    }
}
