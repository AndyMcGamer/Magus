using Magus.PlayerController;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.Skills
{
    public class SkillsBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerHUD playerHUD;
        private SkillDisplayHover[] skillHovers;

        private void Awake()
        {
            skillHovers = GetComponentsInChildren<SkillDisplayHover>();
        }

        private void OnEnable()
        {
            foreach (var hover in skillHovers)
            {
                hover.OnStartDrag += Hover_OnStartDrag;
                hover.OnStopDrag += Hover_OnStopDrag;
                hover.OnHover += Hover_OnHover;
                hover.OnStopHover += Hover_OnStopHover;
            }
        }

        private void OnDisable()
        {
            foreach (var hover in skillHovers)
            {
                hover.OnStartDrag -= Hover_OnStartDrag;
                hover.OnStopDrag -= Hover_OnStopDrag;
                hover.OnHover -= Hover_OnHover;
                hover.OnStopHover -= Hover_OnStopHover;
            }
        }

        private void Hover_OnStartDrag(SkillDisplayHover hover)
        {
            foreach (var skillHover in skillHovers)
            {
                skillHover.SetSelectionState(false);
            }
        }

        private void Hover_OnStopDrag(SkillDisplayHover hover)
        {
            foreach (var skillHover in skillHovers)
            {
                skillHover.SetSelectionState(true);
            }
        }

        private void Hover_OnHover(SkillDisplayHover hover)
        {
            playerHUD.StartDisplaySkill(hover.gameObject, hover.displaySlot);
        }

        private void Hover_OnStopHover()
        {
            playerHUD.StopDisplaySkill();
        }
    }
}
