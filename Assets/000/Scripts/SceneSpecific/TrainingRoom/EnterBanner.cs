using DG.Tweening;
using FishNet.Connection;
using FishNet.Object;
using Magus.Game;
using Magus.UserInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magus.SceneSpecific
{
    public class EnterBanner : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            EnterSequence();
            RoundController.instance.OnGameStageChanged += Transition;
        }

        private void OnDestroy()
        {
            RoundController.instance.OnGameStageChanged -= Transition;
        }

        private async void EnterSequence()
        {

            switch (MatchController.instance.gameMode)
            {
                case GameMode.Training:
                    Banner.instance.SetText("Practice Room");
                    break;
                case GameMode.Standard:
                    Banner.instance.SetText("Equip Your Skills");
                    break;
            }
            await Banner.instance.FadeIn(0.01f);
            await Fader.instance.FadeOut(easeFunction: Ease.InOutQuad);
            await Banner.instance.FadeOut(1.25f, Ease.InQuart);
        }

        private async void Transition()
        {
            Banner.instance.SetText("Prepare for Battle");
            await Banner.instance.FadeIn(0.5f, Ease.OutQuad, false);
            await Banner.instance.FadeOut(1f, Ease.InCubic, false);
        }
    }
}
