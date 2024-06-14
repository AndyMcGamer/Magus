using Magus.Game;
using Magus.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Magus.Skills
{
    public class SkillPointButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SkillUI skillUI;
        [SerializeField] private bool addButton;

        private bool processSkillChange;

        private void OnEnable()
        {
            skillUI.OnSkillUpdate += OnSkillUIButtonPress;
            GlobalPlayerController.instance.OnSkillPointsChanged += OnSkillPointsChanged;
        }

        

        private void OnDisable()
        {
            skillUI.OnSkillUpdate -= OnSkillUIButtonPress;
            GlobalPlayerController.instance.OnSkillPointsChanged -= OnSkillPointsChanged;
        }

        private void OnSkillUIButtonPress(bool add)
        {
            processSkillChange = add == addButton;
        }

        private void OnSkillPointsChanged(int playerNumber, int prev, int next)
        {
            if (!processSkillChange || ConnectionManager.instance.playerData[skillUI.PlayerInfo.LocalConnection] != playerNumber) return;
            processSkillChange = false;
            Clicked(addButton);
        }

        public void Clicked(bool add)
        {
            int next_level = skillUI.SkillLevel + (add ? 1 : -1);
            if (next_level <= 0 || next_level > skillUI.skillNode.skillData.MaxLevel)
            {
                skillUI.PlayerInfo.playerHUD.StopDisplaySkill();
                return;
            }
            skillUI.PlayerInfo.playerHUD.StartDisplaySkill(gameObject, skillUI, next_level, SkillInfoDisplay.SkillDisplayMode.SkillButton);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if ((skillUI.SkillLevel <= 1 && !addButton) || (skillUI.SkillLevel >= skillUI.skillNode.skillData.MaxLevel && addButton)) return;
            skillUI.PlayerInfo.playerHUD.StartDisplaySkill(gameObject, skillUI, skillUI.SkillLevel + (addButton ? 1 : -1), SkillInfoDisplay.SkillDisplayMode.SkillButton);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            skillUI.PlayerInfo.playerHUD.StopDisplaySkill();
        }
    }
}
