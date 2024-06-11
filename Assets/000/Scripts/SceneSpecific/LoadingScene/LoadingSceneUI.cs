using Magus.UserInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class LoadingSceneUI : MonoBehaviour
    {
        [SerializeField] private GameLoader gameLoader;
        [SerializeField] private TextMeshProUGUI countdownText;


        private void Awake()
        {
            EnterScene();
        }

        private void OnEnable()
        {
            gameLoader.OnCountdownChanged += UpdateCountdown;
        }

        private void OnDisable()
        {
            gameLoader.OnCountdownChanged -= UpdateCountdown;
        }

        private void UpdateCountdown(float countdownValue)
        {
            countdownText.text = Mathf.RoundToInt(countdownValue).ToString();
        }

        private async void EnterScene()
        {
            await Fader.instance.FadeOut(0.75f, DG.Tweening.Ease.InOutSine);
        }
    }
}
