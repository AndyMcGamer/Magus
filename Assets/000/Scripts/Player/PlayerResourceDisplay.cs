using DG.Tweening;
using Magus.Game;
using Magus.Multiplayer;
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
        }

        [SerializeField, ReorderableList] private HealthbarUI[] healthbars;

        [SerializeField, Foldout("UsernameColor")] private Color localColor;
        [SerializeField, Foldout("UsernameColor")] private Color remoteColor;

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
                if(hb.Initialize(ConnectionManager.instance.playerData[base.Owner])) healthUI = hb;
            }

            if (base.LocalConnection.Scenes.Contains(gameObject.scene))
            {
                GlobalPlayerController.instance.OnPlayerHealthChange += OnPlayerHealthChange;
                bool localPlayer = base.IsOwner;
                healthUI.SetUsernameColor(localPlayer ? localColor : remoteColor);
                updateHealthbar = true;
            }

            if (!base.IsOwner) return;
        }

        private void OnDestroy()
        {
            if (updateHealthbar)
            {
                GlobalPlayerController.instance.OnPlayerHealthChange -= OnPlayerHealthChange;
            }
        }

        private void OnPlayerHealthChange(int playerNumber, float next)
        {
            if (playerNumber != healthUI.playerNumber) return;

            float fillAmount = next / 100f;
            healthUI.UpdateHealth(fillAmount);
        }
    }
}
