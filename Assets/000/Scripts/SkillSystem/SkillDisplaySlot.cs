using DG.Tweening;
using Magus.Game;
using Magus.Multiplayer;
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
        [SerializeField] private PlayerSkillManager skillManager;
        [SerializeField] private InputProcessor inputProcessor;
        [SerializeField] private int skillNumber;
        [SerializeField] private DraggableSkill iconMask;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI keyPrompt;
        [SerializeField] private GameObject disabledImage;
        [SerializeField] private Image cooldownImage;
        [SerializeField] private TextMeshProUGUI cooldownText;

        private ActiveSkill currentActiveSkill;

        private string ActionName => $"Skill_{skillNumber}";
        private int Index => skillNumber - 1;

        private int SkillLevel => GlobalPlayerController.instance.GetSkillStatus(ConnectionManager.instance.playerData[skillManager.LocalConnection])[currentActiveSkill.skillData.Name];

        private void Awake()
        {
            disabledImage.SetActive(false);
            cooldownImage.fillAmount = 0;
            cooldownText.text = "";
        }

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
                currentActiveSkill = null;
                return;
            }
            currentActiveSkill = skillManager.GetActiveSkill(skillName);
            var skillData = currentActiveSkill.skillData;
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

        private void Update()
        {
            if(currentActiveSkill != null)
            {
                if(currentActiveSkill.cooldown > 0)
                {
                    if (!disabledImage.activeSelf)
                    {
                        disabledImage.SetActive(true);
                    }
                    cooldownImage.fillAmount = currentActiveSkill.cooldown / currentActiveSkill.skillData.Cooldown[SkillLevel - 1];
                    cooldownText.text = ProcessCooldownText(currentActiveSkill.cooldown);
                }
                else
                {
                    if (disabledImage.activeSelf) disabledImage.SetActive(false);
                    cooldownImage.fillAmount = 0;
                    cooldownText.text = "";
                }
            }
        }

        private string ProcessCooldownText(float cooldown)
        {
            if((int)cooldown / 60 > 0)
            {
                return $"{(int)cooldown / 60}min";
            }
            else if(cooldown >= 1)
            {
                return $"{(int)cooldown}sec";
            }
            else
            {
                return cooldown.ToString("F1");
            }
        }
    }
}
