using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magus.Skills
{
    public class SkillInfoDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SkillUI skillUI;
        public void OnPointerEnter(PointerEventData eventData)
        {
            skillUI.ToggleBorder(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            skillUI.ToggleBorder(false);
        }
    }
}
