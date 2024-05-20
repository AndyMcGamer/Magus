using FishNet.Object;
using Magus.Game;
using Magus.Multiplayer;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    public class HealthBarHUD : NetworkBehaviour
    {
        [SerializeField] private Image healthbar_PlayerOne;
        [SerializeField] private Image healthbar_PlayerTwo;

        [SerializeField, Foldout("HP Colors")] private Color friendlyColor;
        [SerializeField, Foldout("HP Colors")] private Color enemyColor;

        public override void OnStartClient()
        {
            base.OnStartClient();
            GlobalPlayerController.instance.OnPlayerHealthChange += OnPlayerHealthChange;
            int playerNumber = ConnectionManager.instance.playerData[LocalConnection];
            OnPlayerHealthChange(1, GlobalPlayerController.instance.GetCurrentHealth(1));
            OnPlayerHealthChange(2, GlobalPlayerController.instance.GetCurrentHealth(2));
            if(playerNumber == 1)
            {
                healthbar_PlayerOne.color = friendlyColor;
                healthbar_PlayerTwo.color = enemyColor;
            }
            else if(playerNumber == 2)
            {
                healthbar_PlayerOne.color = enemyColor;
                healthbar_PlayerTwo.color = friendlyColor;
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
                healthbar_PlayerOne.fillAmount = Mathf.Max(0f, health / 100f);
            }
            else if(playerNumber == 2)
            {
                healthbar_PlayerTwo.fillAmount = Mathf.Max(0f, health / 100f);
            }
        }
    }
}
