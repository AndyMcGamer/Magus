using DG.Tweening;
using FishNet.Object;
using FishNet.Transporting.Multipass;
using Magus.Multiplayer;
using Magus.UserInterface;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.PlayerController
{
    public class PauseScreenManager : NetworkBehaviour
    {
        public enum PauseScreen
        {
            Main,
            Settings,
            QuitConfirm
        }

        [SerializeField] private CanvasGroup pauseScreen;
        [SerializeField, ReadOnly] private PauseScreen currentScreen;

        [SerializeField] private Image quitConfirmBackground;
        [SerializeField] private Transform quitConfirmPanel;

        [SerializeField] private Image controlsBackground;
        [SerializeField] private Transform controlsPanel;

        private bool showingPauseScreen;

        private void Awake()
        {
            currentScreen = PauseScreen.Main;

            pauseScreen.alpha = 0f;
            pauseScreen.blocksRaycasts = false;
            showingPauseScreen = false;

            quitConfirmBackground.DOFade(0f, 0.01f);
            quitConfirmBackground.raycastTarget = false;
            quitConfirmPanel.localScale = Vector3.zero;


            controlsBackground.DOFade(0f, 0.01f);
            controlsBackground.raycastTarget = false;
            controlsPanel.localScale = Vector3.zero;
        }

        public void OnEscapeKey(PlayerControllerInfo playerInfo)
        {
            switch (currentScreen)
            {
                case PauseScreen.Main:
                    showingPauseScreen = !showingPauseScreen;
                    playerInfo.inputProcessor.TogglePlayerInput(!showingPauseScreen);
                    float targetAlpha = showingPauseScreen ? 1f : 0f;
                    pauseScreen.DOFade(targetAlpha, 0.5f).SetEase(Ease.OutSine);
                    pauseScreen.blocksRaycasts = showingPauseScreen;
                    break;
                case PauseScreen.Settings:

                    break;
                case PauseScreen.QuitConfirm:
                    HideQuitConfirm();
                    break;
            }
        }

        public void ShowQuitConfirm()
        {
            currentScreen = PauseScreen.QuitConfirm;
            quitConfirmBackground.DOFade(1f, 0.35f).SetEase(Ease.OutSine);
            quitConfirmBackground.raycastTarget = true;
            quitConfirmPanel.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutSine);
        }

        public void HideQuitConfirm()
        {
            currentScreen = PauseScreen.Main;
            quitConfirmBackground.DOFade(0f, 0.35f).SetEase(Ease.OutSine);
            quitConfirmBackground.raycastTarget = false;
            quitConfirmPanel.DOScale(Vector3.zero, 0.35f).SetEase(Ease.OutSine);
        }

        public void ShowControls()
        {
            currentScreen = PauseScreen.Settings;
            controlsBackground.DOFade(1f, 0.35f).SetEase(Ease.OutSine);
            controlsBackground.raycastTarget = true;
            controlsPanel.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutSine);
        }

        public void HideControls()
        {
            currentScreen = PauseScreen.Main;
            controlsBackground.DOFade(0f, 0.35f).SetEase(Ease.OutSine);
            controlsBackground.raycastTarget = false;
            controlsPanel.DOScale(Vector3.zero, 0.35f).SetEase(Ease.OutSine);
        }

        public void QuitToMenu()
        {
            ConnectionManager.instance.ForceDisconnectClient();
        }

        public void QuitGame()
        {
            QuitToMenu();
            Application.Quit();
        }
        
    }
}
