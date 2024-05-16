using Magus.Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class RoundTimerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundTimerText;
        private void Awake()
        {
            RoundController.instance.OnStageTimerChanged += StageTimerChanged;
        }

        private void OnDestroy()
        {
            RoundController.instance.OnStageTimerChanged -= StageTimerChanged;
        }

        private void StageTimerChanged(float time)
        {
            int minutes = (int)time / 60;
            int seconds = (int)time % 60;
            string timeString = $"{minutes:00}:{seconds:00}";
            roundTimerText.text = timeString;
        }
    }
}
