using FishNet.Object;
using Magus.MatchmakingSystem;
using Magus.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    public class EndSceneUI : MonoBehaviour
    {
        [SerializeField] private GameEnd gameEnd;

        [SerializeField] private TextMeshProUGUI winnerName;
        [SerializeField] private TextMeshProUGUI outcomeText;
        [SerializeField] private Button quitButton;

        private bool quitting;

        private void Awake()
        {
            quitButton.interactable = false;
            var images = quitButton.GetComponentsInChildren<Image>();
            var text = quitButton.GetComponentInChildren<TextMeshProUGUI>();
            foreach (var image in images)
            {
                image.color = quitButton.colors.disabledColor;
            }
            text.color = quitButton.colors.disabledColor;
        }

        private void OnEnable()
        {
            gameEnd.OnCountdownChanged += GameEnd_OnCountdownChanged;
            gameEnd.OnLoadWinner += LoadWinner;
        }

        private void OnDisable()
        {
            gameEnd.OnCountdownChanged -= GameEnd_OnCountdownChanged;
            gameEnd.OnLoadWinner -= LoadWinner;
        }

        private void LoadWinner(int playerNumber, int winnerNumber)
        {
            print("WinnerLoaded");
            quitButton.interactable = true;
            var images = quitButton.GetComponentsInChildren<Image>();
            var text = quitButton.GetComponentInChildren<TextMeshProUGUI>();
            foreach (var image in images)
            {
                image.color = Color.white;
            }
            text.color = Color.white;
            string winner = ConnectionManager.instance.playerNames[winnerNumber];
            winnerName.text = winner;
            if(playerNumber == winnerNumber)
            {
                outcomeText.text = "Won";
            }
            else
            {
                outcomeText.text = "Lost";
            }
        }

        private void GameEnd_OnCountdownChanged(float timer)
        {
            print(timer);
            if (timer <= 0)
            {
                QuitToLobby();
            }
        }

        public void QuitToLobby()
        {
            if (quitting) return;

            print("Quit");

            quitting = true;
            LobbyManager.instance.UpdateGameStatus(false);

            // Temporary -- should actually return to lobby
            Application.Quit();
        }
    }
}