using Magus.Global;
using Magus.MatchmakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("Player One")]
        [SerializeField] private GameObject playerData1;
        [SerializeField] private TextMeshProUGUI usernameText1;
        [SerializeField] private GameObject hostIcon1;
        [SerializeField] private GameObject kickIcon1;
        [SerializeField] private GameObject readyIcon1;

        [Header("Player Two")]
        [SerializeField] private GameObject playerData2;
        [SerializeField] private TextMeshProUGUI usernameText2;
        [SerializeField] private GameObject hostIcon2;
        [SerializeField] private GameObject kickIcon2;
        [SerializeField] private GameObject readyIcon2;

        [Header("Host Controls")]
        [SerializeField] private GameObject playerToggle;
        [SerializeField] private GameObject duelIcon;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button toggleButton;

        [Header("Shared")]
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI joinCodeText;
        [SerializeField] private Text readyButtonText;

        private void OnEnable()
        {
            LobbyManager.instance.OnLobbyUpdate += LobbyUpdate;
            LobbyManager.instance.OnLobbyKicked += LobbyKicked;
            LobbyManager.instance.OnLeftLobby += LeftLobby;
            LobbyManager.instance.OnLobbyDeleted += LobbyDeleted;
            LobbyManager.instance.OnGameStarted += GameStarted;
        }

        private void OnDisable()
        {
            LobbyManager.instance.OnLobbyUpdate -= LobbyUpdate;
            LobbyManager.instance.OnLobbyKicked -= LobbyKicked;
            LobbyManager.instance.OnLeftLobby -= LeftLobby;
            LobbyManager.instance.OnLobbyDeleted -= LobbyDeleted;
            LobbyManager.instance.OnGameStarted -= GameStarted;
        }

        private void Awake()
        {
            UpdateLobbyUI();
        }

        private void UpdateLobbyUI()
        {
            if (LobbyManager.instance.IsHost)
            {
                startButton.interactable = true;
                startButton.GetComponentInChildren<Text>().text = "Start Game";
                playerToggle.SetActive(true);
                duelIcon.SetActive(false);
            }
            else
            {
                startButton.interactable = false;
                startButton.GetComponentInChildren<Text>().text = "Waiting for Host";
                playerToggle.SetActive(false);
                duelIcon.SetActive(true);
            }
            readyButtonText.text = bool.Parse(LobbyManager.instance.LocalPlayer.Data["ReadyCheck"].Value) ? "Cancel" : "Ready";
            lobbyNameText.text = LobbyManager.instance.Lobby.Name;
            joinCodeText.text = $"Join Code: {LobbyManager.instance.LobbyCode}";

            kickIcon1.SetActive(false);
            kickIcon2.SetActive(false);

            playerData1.SetActive(false);
            playerData2.SetActive(false);

            bool allPlayersReady = true;

            foreach (var player in LobbyManager.instance.Lobby.Players)
            {
                if (player.Data["PlayerNumber"].Value == "1")
                {
                    playerData1.SetActive(true);
                    if (player.Id == LobbyManager.instance.Lobby.HostId)
                    {
                        hostIcon1.SetActive(true);
                        kickIcon1.SetActive(false);
                    }
                    else
                    {
                        hostIcon1.SetActive(false);
                        if(LobbyManager.instance.IsHost) kickIcon1.SetActive(true);
                    }
                    
                    readyIcon1.SetActive(bool.Parse(player.Data["ReadyCheck"].Value));
                    usernameText1.text = player.Data["PlayerName"].Value;
                }

                if (player.Data["PlayerNumber"].Value == "2")
                {
                    playerData2.SetActive(true);
                    if (player.Id == LobbyManager.instance.Lobby.HostId)
                    {
                        hostIcon2.SetActive(true);
                        kickIcon2.SetActive(false);
                    }
                    else
                    {
                        hostIcon2.SetActive(false);
                        if (LobbyManager.instance.IsHost) kickIcon2.SetActive(true);
                    }

                    readyIcon2.SetActive(bool.Parse(player.Data["ReadyCheck"].Value));
                    usernameText2.text = player.Data["PlayerName"].Value;
                }

                if (!bool.Parse(player.Data["ReadyCheck"].Value))
                {
                    allPlayersReady = false;
                }

                print($"{player.Data["PlayerNumber"].Value} {player.Id} {LobbyManager.instance.Lobby.HostId}");
            }

            if (LobbyManager.instance.IsHost) startButton.interactable = (allPlayersReady && LobbyManager.instance.Lobby.Players.Count == Constants.MAX_PLAYERS);
        }

        private void LobbyUpdate(object sender, LobbyManager.LobbyEventArgs e)
        {
            UpdateLobbyUI();
        }

        private void LobbyKicked(object sender, LobbyManager.LobbyEventArgs e)
        {
            
        }

        private void LeftLobby(object sender, EventArgs e)
        {
            
        }

        private void LobbyDeleted(object sender, EventArgs e)
        {

        }

        private void GameStarted(object sender, EventArgs e)
        {

        }

        public void ToggleReady()
        {
            bool ready = !bool.Parse(LobbyManager.instance.LocalPlayer.Data["ReadyCheck"].Value);
            LobbyManager.instance.UpdateReadyCheck(ready);
        }

        public async void TogglePlayerSides()
        {
            toggleButton.interactable = false;
            await LobbyManager.instance.TogglePlayerNumbers();
            toggleButton.interactable = true;
        }

        public async void StartGame()
        {
            bool allPlayersReady = true;
            foreach (var player in LobbyManager.instance.Lobby.Players)
            {
                if (!bool.Parse(player.Data["ReadyCheck"].Value))
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (!allPlayersReady || LobbyManager.instance.Lobby.Players.Count != Constants.MAX_PLAYERS)
            {
                return;
            }

            await LobbyManager.instance.StartGame();
        }

    }
}