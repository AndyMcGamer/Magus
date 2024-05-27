using Magus.Game;
using Magus.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.PlayerController
{
    public class PlayerHotbar : PlayerControllerComponent
    {
        [SerializeField] private Image healthBarLocal;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner) return;
            GlobalPlayerController.instance.OnPlayerHealthChange += OnPlayerHealthChange;
            OnPlayerHealthChange(1, GlobalPlayerController.instance.GetCurrentHealth(1));
        }

        private void OnDestroy()
        {
            GlobalPlayerController.instance.OnPlayerHealthChange -= OnPlayerHealthChange;
        }
        private void OnPlayerHealthChange(int playerNumber, float health)
        {
            if (playerNumber != ConnectionManager.instance.playerData[base.LocalConnection]) return;
            healthBarLocal.fillAmount = health / 100f;
        }
    }
}
