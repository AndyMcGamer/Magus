using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerHUD : PlayerControllerComponent
    {
        [Header("References")]
        [SerializeField] private GameObject skillScreen;
        [SerializeField] private GameObject statScreen;

        private bool showingSkillScreen;
        private bool showingStatScreen;

        private void Awake()
        {
            skillScreen.SetActive(false);
            statScreen.SetActive(false);
            showingSkillScreen = false;
            showingStatScreen = false;
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
