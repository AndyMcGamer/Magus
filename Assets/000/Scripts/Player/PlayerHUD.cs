using Magus.Game;
using Magus.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerHUD : PlayerControllerComponent
    {
        [Header("References")]
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject skillScreen;
        [SerializeField] private GameObject statScreen;

        [SerializeField] private TextMeshProUGUI skillPointDisplay;

        private bool showingSkillScreen;
        private bool showingStatScreen;

        private void Awake()
        {
            skillScreen.SetActive(false);
            statScreen.SetActive(false);
            showingSkillScreen = false;
            showingStatScreen = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner) return;
            hud.SetActive(true);
            GlobalPlayerController.instance.OnSkillPointsChanged += OnSkillPointsChanged;
            SetSkillPointText(GlobalPlayerController.instance.GetSkillPoints(ConnectionManager.instance.playerData[base.LocalConnection]));
        }

        private void OnDestroy()
        {
            if (!base.IsOwner) return;
            GlobalPlayerController.instance.OnSkillPointsChanged -= OnSkillPointsChanged;
        }

        private void OnSkillPointsChanged(int playerNumber)
        {
            if (playerNumber != ConnectionManager.instance.playerData[base.LocalConnection]) return;
            SetSkillPointText(GlobalPlayerController.instance.GetSkillPoints(playerNumber));
        }

        private void SetSkillPointText(int skillPoints)
        {
            skillPointDisplay.text = $"Skill Points: {skillPoints}";
        }

        public void ToggleSkillScreen()
        {
            showingSkillScreen = !showingSkillScreen;
            skillScreen.SetActive(showingSkillScreen);
        }

        public void ToggleStatScreen()
        {
            showingStatScreen= !showingStatScreen;
            statScreen.SetActive(showingStatScreen);
        }
    }
}
