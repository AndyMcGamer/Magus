using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Magus.UserInterface
{
    public class Banner : MonoBehaviour
    {
        public static Banner instance;
        [SerializeField] private CanvasGroup bannerGroup;
        [SerializeField] private TextMeshProUGUI bannerText;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task FadeIn(float time = 0.5f, Ease easeFunction = Ease.Linear, bool reset = true)
        {
            bannerGroup.blocksRaycasts = true;
            if (reset) await bannerGroup.DOFade(0f, 0.01f).AsyncWaitForCompletion();
            await bannerGroup.DOFade(1f, time).SetEase(easeFunction).AsyncWaitForCompletion();
        }

        public async Task FadeOut(float time = 0.5f, Ease easeFunction = Ease.Linear, bool reset = true)
        {
            bannerGroup.blocksRaycasts = false;
            if (reset) await bannerGroup.DOFade(1f, 0.01f).AsyncWaitForCompletion();
            await bannerGroup.DOFade(0f, time).SetEase(easeFunction).AsyncWaitForCompletion();
        }

        public async Task FadeInOut(float time = 0.5f, Ease easeFunction = Ease.Linear)
        {
            bannerGroup.blocksRaycasts = true;
            await bannerGroup.DOFade(1f, time).SetEase(easeFunction).AsyncWaitForCompletion();
            await bannerGroup.DOFade(0f, time).SetEase(easeFunction).AsyncWaitForCompletion();
            bannerGroup.blocksRaycasts = false;
        }

        public void SetText(string text)
        {
            bannerText.text = text;
        }
    }
}
