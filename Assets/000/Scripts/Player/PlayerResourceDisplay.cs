using DG.Tweening;
using Magus.Game;
using Magus.Multiplayer;
using Magus.UserInterface;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.PlayerController
{
    public class PlayerResourceDisplay : PlayerControllerComponent
    {
        [System.Serializable]
        public class HealthbarUI
        {
            public int playerNumber;
            [SerializeField] private GameObject healthHolder;
            [SerializeField] private TextMeshProUGUI usernameText;
            [SerializeField] private Image healthBarImage;
            [SerializeField] private Image shadowImage;

            [SerializeField] private GameObject scoreHolder;
            [SerializeField] private Image[] scores;

            public bool Initialize(int playerNumber)
            {
                if(this.playerNumber != playerNumber)
                {
                    healthHolder.SetActive(false);
                    return false;
                }
                healthHolder.SetActive(true);
                usernameText.text = ConnectionManager.instance.playerNames[playerNumber];
                return true;
            }

            public void UpdateHealth(float fillAmount)
            {
                healthBarImage.fillAmount = fillAmount;
                DOTween.To(() => shadowImage.fillAmount, x => shadowImage.fillAmount = x, fillAmount, 0.75f).SetEase(Ease.InQuart);
            }

            public void SetUsernameColor(Color color)
            {
                usernameText.color = color;
            }

            public void ResetScore(Color color)
            {
                foreach (var score in scores)
                {
                    score.color = color;
                }
            }

            public void UpdateScore(Color winColor)
            {
                int score = playerNumber == 1 ? MatchController.instance.wins_PlayerOne : MatchController.instance.wins_PlayerTwo;
                for (int i = 0; i < score; i++)
                {
                    scores[i].DOColor(winColor, 0.5f).SetEase(Ease.InOutSine);
                }
            }

            public void HideScore()
            {
                scoreHolder.SetActive(false);
            }
        }

        [SerializeField, ReorderableList] private HealthbarUI[] healthbars;

        [SerializeField, Foldout("UsernameColor")] private Color localColor;
        [SerializeField, Foldout("UsernameColor")] private Color remoteColor;

        [SerializeField, Foldout("ScoreColor")] private Color winColor;
        [SerializeField, Foldout("ScoreColor")] private Color defaultColor;

        private HealthbarUI healthUI;
        private bool updateHealthbar = false;

        private void Awake()
        {
            updateHealthbar = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            foreach (var hb in healthbars)
            {
                hb.ResetScore(defaultColor);
                if(hb.Initialize(ConnectionManager.instance.playerData[base.Owner])) healthUI = hb;
                hb.UpdateScore(winColor);
            }

            if (base.LocalConnection.Scenes.Contains(gameObject.scene))
            {
                GlobalPlayerController.instance.OnPlayerHealthChange += OnPlayerHealthChange;
                bool localPlayer = base.IsOwner;
                healthUI.SetUsernameColor(localPlayer ? localColor : remoteColor);
                
                updateHealthbar = true;

                if(MatchController.instance.gameMode != GameMode.Standard)
                {
                    healthUI.HideScore();
                }
                
            }

            RoundController.instance.OnRoundWinner += UpdateScore;

            if (!base.IsOwner) return;
            
        }

        private void OnDestroy()
        {
            if (updateHealthbar)
            {
                GlobalPlayerController.instance.OnPlayerHealthChange -= OnPlayerHealthChange;
                
            }

            RoundController.instance.OnRoundWinner -= UpdateScore;
        }

        private void OnPlayerHealthChange(int playerNumber, float next)
        {
            if (playerNumber != healthUI.playerNumber) return;

            float fillAmount = next / 100f;
            healthUI.UpdateHealth(fillAmount);
        }

        public async void UpdateScore(int winner)
        {
            if (healthUI == null) return;

            healthUI.UpdateScore(winColor);

            if (!base.IsOwner) return;

            if (winner == 0)
            {
                Banner.instance.SetText("Round Tie");
                await Banner.instance.FadeIn(1f, easeFunction: Ease.OutQuad);
                await Banner.instance.FadeOut(1.25f, easeFunction: Ease.InQuart);
            }
            else if (healthUI.playerNumber != winner)
            {
                Banner.instance.SetText("Round Lost");
                await Banner.instance.FadeIn(1f, easeFunction: Ease.OutQuad);
                await Banner.instance.FadeOut(1.25f, easeFunction: Ease.InQuart);
            }
            else
            {
                Banner.instance.SetText("Round Won");
                await Banner.instance.FadeIn(1f, easeFunction: Ease.OutQuad);
                await Banner.instance.FadeOut(1.25f, easeFunction: Ease.InQuart);
            }
            

        }
    }
}
