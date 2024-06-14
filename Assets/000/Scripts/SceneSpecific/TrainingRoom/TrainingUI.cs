using DG.Tweening;
using FishNet.Object;
using Magus.Game;
using Magus.Global;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    public class TrainingUI : NetworkBehaviour
    {

        [Header("Ready Check")]
        [SerializeField] private RectTransform readyPanel;
        [SerializeField] private RectTransform readyHolder;
        [SerializeField] private RectTransform expandPanel;
        [SerializeField] private GameObject expandButton;
        [SerializeField] private GameObject retractButton;

        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private Image buttonFrame;

        private Vector2 RetractedPosition => (readyHolder.rect.width) * Vector2.left;
        private Vector2 ExpandedPosition => Vector2.zero;

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!base.IsOwner || MatchController.instance.gameMode != GameMode.Standard || RoundController.instance.gameStage != GameStage.Training)
            {
                readyPanel.gameObject.SetActive(false);
                return;
            }

            readyPanel.gameObject.SetActive(true);

            RoundController.instance.OnReadyPlayersChanged += OnReadyPlayersChanged;

            readyButton.interactable = true;
            buttonFrame.DOFade(1f, 0.01f);
            expandButton.SetActive(true);
            retractButton.SetActive(false);
            readyPanel.anchoredPosition = RetractedPosition;

            OnReadyPlayersChanged(RoundController.instance.readyPlayers);
        }

        private void OnDestroy()
        {
            if(base.IsOwner) RoundController.instance.OnReadyPlayersChanged -= OnReadyPlayersChanged;
        }

        private void OnReadyPlayersChanged(int readyPlayers)
        {
            readyText.text = $"Ready ({readyPlayers}/{MatchController.instance.MinPlayers})";
        }

        public void ReadyUp()
        {
            RoundController.instance.AddReadyPlayer();
            readyButton.interactable = false;
            buttonFrame.DOFade(readyButton.colors.disabledColor.a, 0.35f);
        }

        public void Expand()
        {
            expandButton.SetActive(false);
            retractButton.SetActive(true);
            DOTween.To(() => readyPanel.anchoredPosition, x => readyPanel.anchoredPosition = x, ExpandedPosition, 0.35f).SetEase(Ease.InOutSine);
        }

        public void Retract()
        {
            expandButton.SetActive(true);
            retractButton.SetActive(false);
            DOTween.To(() => readyPanel.anchoredPosition, x => readyPanel.anchoredPosition = x, RetractedPosition, 0.35f).SetEase(Ease.InOutSine);
        }
    }
}
