using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Magus.Skills
{
    public class SkillIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SkillUI skillUI;

        private bool hovering;
        public void OnPointerEnter(PointerEventData eventData)
        {
            skillUI.ToggleBorder(true);
            hovering = true;
            skillUI.PlayerInfo.playerHUD.StartDisplaySkill(gameObject, skillUI, skillUI.SkillLevel, SkillInfoDisplay.SkillDisplayMode.SkillTree);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            skillUI.ToggleBorder(false);
            hovering = false;
            skillUI.PlayerInfo.playerHUD.StopDisplaySkill();
        }

        private void OnDisable()
        {
            if(hovering) skillUI.PlayerInfo.playerHUD.StopDisplaySkill();
        }
    }
}
