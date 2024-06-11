using DG.Tweening;
using FishNet.Object;
using Magus.Game;
using Magus.Multiplayer;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    public class HealthBarHUD : NetworkBehaviour
    {
        [SerializeField] private Image healthbar_PlayerOne;
        [SerializeField] private Image healthbar_PlayerTwo;
        [SerializeField] private Image shadow_PlayerOne;
        [SerializeField] private Image shadow_PlayerTwo;
        [SerializeField] private TextMeshProUGUI username_PlayerOne;
        [SerializeField] private TextMeshProUGUI username_PlayerTwo;

        [SerializeField, Foldout("Username")] private Color localColor;
        [SerializeField, Foldout("Username")] private Color otherColor;

        //private Tweener tweener_PlayerOne;
        //private Tweener tweener_PlayerTwo;

        public override void OnStartClient()
        {
            base.OnStartClient();
            GlobalPlayerController.instance.OnPlayerHealthChange += OnPlayerHealthChange;
            int playerNumber = ConnectionManager.instance.playerData[LocalConnection];
            OnPlayerHealthChange(1, GlobalPlayerController.instance.GetCurrentHealth(1));
            OnPlayerHealthChange(2, GlobalPlayerController.instance.GetCurrentHealth(2));
            username_PlayerOne.text = ConnectionManager.instance.playerNames[1];
            username_PlayerTwo.text = ConnectionManager.instance.playerNames[2];
            if (playerNumber == 1)
            {
                username_PlayerOne.color = localColor;
                username_PlayerTwo.color = otherColor;
            }
            else if(playerNumber == 2)
            {
                username_PlayerTwo.color = localColor;
                username_PlayerOne.color = otherColor;
            }
        }

        private void OnDestroy()
        {
            GlobalPlayerController.instance.OnPlayerHealthChange -= OnPlayerHealthChange;
        }

        private void OnPlayerHealthChange(int playerNumber, float health)
        {
            if(playerNumber == 1)
            {
                //tweener_PlayerOne?.Kill();

                float fillAmount = health / 100f;

                DOTween.To(() => shadow_PlayerOne.fillAmount, x => shadow_PlayerOne.fillAmount = x, fillAmount, 0.65f).SetEase(Ease.InQuart);
                healthbar_PlayerOne.fillAmount = health / 100f;
            }
            else if(playerNumber == 2)
            {
                //tweener_PlayerTwo?.Kill();

                float fillAmount = health / 100f;

                DOTween.To(() => shadow_PlayerTwo.fillAmount, x => shadow_PlayerTwo.fillAmount = x, fillAmount, 0.65f).SetEase(Ease.InQuart);
                healthbar_PlayerTwo.fillAmount = fillAmount;
            }
        }
    }
}
