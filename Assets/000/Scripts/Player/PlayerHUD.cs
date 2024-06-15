using DG.Tweening;
using FishNet.Object;
using Magus.Game;
using Magus.Multiplayer;
using Magus.Skills;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.PlayerController
{
    public class PlayerHUD : PlayerControllerComponent
    {
        [Header("References")]
        [SerializeField] private SkillInfoDisplay skillInfoDisplay;
        [SerializeField] private PauseScreenManager pauseManager;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject skillScreen;
        [SerializeField] private GameObject statScreen;

        [Header("Reset Skills")]
        [SerializeField] private CanvasGroup resetSkillGroup;
        [SerializeField] private Button resetButton;

        [SerializeField] private TextMeshProUGUI skillPointDisplay;

        private bool showingSkillScreen;
        private bool showingStatScreen;
        

        private void Awake()
        {
            skillScreen.SetActive(false);
            statScreen.SetActive(false);
            
            showingSkillScreen = false;
            showingStatScreen = false;
            
            hud.SetActive(false);
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
            GlobalPlayerController.instance.OnSkillPointsChanged -= OnSkillPointsChanged;
        }

        private void OnSkillPointsChanged(int playerNumber, int prev, int next)
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

        public void TogglePauseScreen()
        {
            pauseManager.OnEscapeKey(playerInfo);
        }

        public void StartDisplaySkill(GameObject skillIconObject, SkillUI skillUI, int skillLevel, SkillInfoDisplay.SkillDisplayMode mode)
        {
            skillInfoDisplay.ShowSkillInfo(skillIconObject, skillUI.PlayerInfo.skillManager, skillUI.skillNode, skillLevel, mode);
        }

        public void StartDisplaySkill(GameObject skillIconObject, SkillDisplaySlot displaySlot)
        {
            skillInfoDisplay.ShowSkillInfo_Hotbar(skillIconObject, displaySlot);
        }

        public void StopDisplaySkill()
        {
            skillInfoDisplay.HideSkillInfo();
        }

        [Client]
        public void ResetSkills()
        {
            if (GlobalPlayerController.instance.RefundSkills() && MatchController.instance.gameMode == GameMode.Standard)
            {
                resetButton.interactable = false;
                resetSkillGroup.DOFade(0.75f, 0.3f);
            }
        }
    }
}
