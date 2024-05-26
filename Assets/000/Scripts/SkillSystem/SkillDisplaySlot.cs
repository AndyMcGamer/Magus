using Magus.Game;
using Magus.PlayerController;
using Magus.UserInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magus.Skills
{
    public class SkillDisplaySlot : MonoBehaviour, IDropHandler
    {
        [SerializeField] private InputProcessor inputProcessor;
        [SerializeField] private int skillNumber;
        [SerializeField] private DraggableSkill iconMask;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI keyPrompt;


        private string ActionName => $"Skill_{skillNumber}";
        private int Index => skillNumber - 1;

        private void OnEnable()
        {
            inputProcessor.OnLoadedProcessor += InputProcessor_OnLoadedProcessor;
            GlobalPlayerController.instance.OnHotbarUpdated += OnHotbarUpdated;
            OnHotbarUpdated(Index);
            keyPrompt.text = inputProcessor.GetActionName(ActionName);
        }

        private void OnDisable()
        {
            GlobalPlayerController.instance.OnHotbarUpdated -= OnHotbarUpdated;
            inputProcessor.OnLoadedProcessor -= InputProcessor_OnLoadedProcessor;
        }

        private void InputProcessor_OnLoadedProcessor()
        {
            keyPrompt.text = inputProcessor.GetActionName(ActionName);
        }

        private void OnHotbarUpdated(int hotbarIndex)
        {
            if (Index != hotbarIndex) return;
            string skillName = GlobalPlayerController.instance.hotbarSkills[Index];
            iconMask.ResetPosition();
            if (skillName == default)
            {
                iconMask.gameObject.SetActive(false);
                skillIcon.sprite = null;
                return;
            }
            var skillData = GlobalPlayerController.instance.skillDatabase.FindDataByName<SkillData>(skillName);
            iconMask.gameObject.SetActive(true);
            skillIcon.sprite = skillData.Icon;
        }

        public void OnDrop(PointerEventData eventData)
        {
            eventData.pointerDrag.TryGetComponent<DraggableSkill>(out var droppedObj);
            if (droppedObj == null) return;
            var otherDisplaySlot = droppedObj.originalParent.GetComponent<SkillDisplaySlot>();
            GlobalPlayerController.instance.SwapSkills(Index, otherDisplaySlot.Index);
        }
    }
}
