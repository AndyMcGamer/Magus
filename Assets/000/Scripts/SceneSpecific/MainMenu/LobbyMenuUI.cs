using Magus.MatchmakingSystem;
using Magus.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    public class LobbyMenuUI : MonoBehaviour
    {
        [Header("Host Lobby")]
        [SerializeField] private TMP_InputField lobbyNameText;
        [SerializeField] private Toggle privateToggle;
        [SerializeField] private TextMeshProUGUI hostErrorText;

        [Header("Join Lobby")]
        [SerializeField] private TextMeshProUGUI joinErrorText;
        [SerializeField] private TMP_InputField joinCodeText;

        private void Awake()
        {
            ResetScreens();
        }

        public async void RequestCreateLobby()
        {
            await Fader.instance.FadeIn(0.75f, DG.Tweening.Ease.InSine);

            string defaultLobbyName = $"{PlayerInfoManager.instance.PlayerInfo.username}'s Lobby";
            if (defaultLobbyName.Length > 30) defaultLobbyName = PlayerInfoManager.instance.PlayerInfo.username[..22] + "'s Lobby";
            string lobbyName = string.IsNullOrWhiteSpace(lobbyNameText.text) ? defaultLobbyName : lobbyNameText.text;

            if (lobbyName.Length > 30) lobbyName = lobbyName[..30];
            
            LobbyServiceException e = await LobbyManager.instance.CreateLobbyInstance(lobbyName, privateToggle.isOn);

            if (e != null)
            {
                hostErrorText.text = e.ErrorCode switch
                {
                    0 => "unknown error occured",
                    2 => "request timed out",
                    3 => "service down, try again later",
                    16007 => "lobby already exists",
                    _ => "an error occured with your request",
                };
                await Fader.instance.FadeOut(0.75f, DG.Tweening.Ease.OutSine);
            }
        }

        public async void RequestJoinLobby()
        {
            await Fader.instance.FadeIn(0.75f, DG.Tweening.Ease.InSine);

            LobbyServiceException e = await LobbyManager.instance.JoinLobbyInstance(joinCodeText.text);
            if(e != null)
            {
                joinErrorText.text = e.ErrorCode switch
                {
                    0 => "unknown error occured",
                    2 => "request timed out",
                    3 => "service down, try again later",
                    16001 => "lobby not found",
                    16010 => "error: invalid join code",
                    _ => "an error occured with your request",
                };
                await Fader.instance.FadeOut(0.5f, DG.Tweening.Ease.OutSine);
            }
        }

        public void ResetScreens()
        {
            lobbyNameText.text = "";
            joinCodeText.text = "";
            hostErrorText.text = "";
            joinErrorText.text = "";
        }
    }
}
