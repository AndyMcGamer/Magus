using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.UserInterface
{
    public class Fader : MonoBehaviour
    {
        public static Fader instance;
        [SerializeField] private Image image;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task FadeIn(float time = 0.5f, Ease easeFunction = Ease.Linear, bool reset = true)
        {
            image.raycastTarget = true;
            if(reset) await image.DOFade(0f, 0.01f).AsyncWaitForCompletion();
            await image.DOFade(1f, time).SetEase(easeFunction).AsyncWaitForCompletion();
        }

        public async Task FadeOut(float time = 0.5f, Ease easeFunction = Ease.Linear, bool reset = true)
        {
            image.raycastTarget = false;
            if (reset) await image.DOFade(1f, 0.01f).AsyncWaitForCompletion();
            await image.DOFade(0f, time).SetEase(easeFunction).AsyncWaitForCompletion();
        }

        public async Task FadeInOut(float time = 0.5f, Ease easeFunction = Ease.Linear)
        {
            image.raycastTarget = true;
            await image.DOFade(1f, time).SetEase(easeFunction).AsyncWaitForCompletion();
            await image.DOFade(0f, time).SetEase(easeFunction).AsyncWaitForCompletion();
            image.raycastTarget = false;
        }

        public void SetAlpha(float alpha)
        {
            Vector4 col = (Vector4)image.color;
            col.w = alpha;
            image.color = col;
        }
    }
}
