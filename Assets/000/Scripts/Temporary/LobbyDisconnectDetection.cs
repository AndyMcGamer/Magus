using Magus.MatchmakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Temporary
{
    public class LobbyDisconnectDetection : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void OnEnable()
        {
            LobbyManager.instance.OnLeftLobby += Instance_OnLeftLobby;
        }

        private void OnDisable()
        {
            LobbyManager.instance.OnLeftLobby -= Instance_OnLeftLobby;
        }

        private void Instance_OnLeftLobby(object sender, EventArgs e)
        {
            print("Lobby Left");
        }
    }
}
