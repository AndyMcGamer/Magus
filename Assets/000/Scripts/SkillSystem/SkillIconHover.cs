using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Magus.Skills
{
    public class SkillIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SkillUI skillUI;

        public void OnPointerEnter(PointerEventData eventData)
        {
            skillUI.ToggleBorder(true);
            skillUI.PlayerInfo.playerHUD.StartDisplaySkill(gameObject, skillUI, skillUI.SkillLevel, SkillInfoDisplay.SkillDisplayMode.SkillTree);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            skillUI.ToggleBorder(false);
            skillUI.PlayerInfo.playerHUD.StopDisplaySkill();
        }
    }
}
