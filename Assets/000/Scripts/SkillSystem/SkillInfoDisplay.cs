using Magus.Game;
using Magus.Global;
using Magus.PlayerController;
using Magus.Skills.SkillTree;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.Skills
{
    public class SkillInfoDisplay : MonoBehaviour
    {
        public enum SkillDisplayMode
        {
            SkillTree,
            SkillButton,
            Hotbar,
        }

        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform displayPanelTransform;
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI costAndCooldownText;
        [SerializeField] private GameObject requirementsBlock;
        [SerializeField] private TextMeshProUGUI requirementsPrefab;
        [SerializeField] private TextMeshProUGUI skillTypeText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject helperContainer;
        [SerializeField] private TextMeshProUGUI helperText;

        [Header("Settings")]
        [SerializeField] private int maxRequirements;
        [SerializeField, Foldout("RequirementColors")] private Color satisfiedColor;
        [SerializeField, Foldout("RequirementColors")] private Color unsatisfiedColor;
        [SerializeField, Foldout("DisplayOffsets")] private Vector2 treeDisplayOffset;
        [SerializeField, Foldout("DisplayOffsets")] private Vector2 skillButtonOffset;
        [SerializeField, Foldout("DisplayOffsets")] private Vector2 hotbarDisplayOffset;

        private TextMeshProUGUI[] requirementsPool;
        private RectTransform currentIconTransform;
        [ReadOnly, SerializeField] private SkillNode currentSkillNode;

        private Vector2 CanvasSize => canvas.renderingDisplaySize;

        private void Awake()
        {
            displayPanelTransform.gameObject.SetActive(false);
            requirementsPool = new TextMeshProUGUI[maxRequirements];
            for (int i = 0; i < maxRequirements; i++)
            {
                requirementsPool[i] = Instantiate(requirementsPrefab, requirementsBlock.transform);
                requirementsPool[i].gameObject.SetActive(false);
            }
            requirementsBlock.SetActive(false);
        }

        public void ShowSkillInfo_Hotbar(GameObject iconObject, SkillDisplaySlot displaySlot)
        {
            int skillIndex = displaySlot.Index;
            int skillLevel = displaySlot.SkillLevel;

            string skillName = GlobalPlayerController.instance.hotbarSkills[skillIndex];
            SkillNode node = GlobalPlayerController.instance.skillDatabase.FindNodeByName(skillName);

            ShowSkillInfo(iconObject, displaySlot.skillManager, node, skillLevel, SkillDisplayMode.Hotbar);
        }

        public void ShowSkillInfo(GameObject iconObject, PlayerSkillManager skillManager, SkillNode skillNode, int currentSkillLevel, SkillDisplayMode displayMode)
        {
            if (currentSkillNode != null && currentSkillNode.skillData.Name != skillNode.skillData.Name && displayMode != SkillDisplayMode.Hotbar)
            {
                return;
            }

            int skillLevel = currentSkillLevel;
            if (currentSkillLevel == 0) currentSkillLevel = 1;
            currentSkillLevel--;

            currentIconTransform = iconObject.GetComponent<RectTransform>();

            currentSkillNode = skillNode;
            var skillData = currentSkillNode.skillData;

            skillNameText.text = currentSkillNode.skillData.Name;

            skillIcon.sprite = currentSkillNode.skillData.Icon;

            string costString = skillData.Cost[currentSkillLevel] == 0 ? "" : $"{Constants.RESOURCE_NAME} {skillData.Cost[currentSkillLevel] : #} / ";
            string cooldownString = skillData.Cooldown[currentSkillLevel] == 0 ? "" : $"Cooldown: {skillData.Cooldown[currentSkillLevel] : 0.#} Sec";
            costAndCooldownText.text = $"{costString}{cooldownString}";

            levelText.text = $"Level {currentSkillLevel + 1}{(skillLevel == skillData.MaxLevel  ? " | MAX" : "")}";

            
            for (int i = 0; i < currentSkillNode.prerequisites.Count; i++)
            {
                var prereq = currentSkillNode.prerequisites[i];
                bool satisfiedPrereq = skillManager.CheckPrerequisite(prereq);

                requirementsPool[i].color = satisfiedPrereq ? satisfiedColor : unsatisfiedColor;
                requirementsPool[i].text = $"{prereq.skill.Name} [Level {prereq.requiredLevel}+]";
                requirementsPool[i].gameObject.SetActive(true);
            }
            if (currentSkillNode.prerequisites.Count > 0) requirementsBlock.SetActive(true);

            skillTypeText.text = currentSkillNode.skillData is PassiveSkillData ? "Passive" : "Active";

            descriptionText.text = skillData.GetDescription(currentSkillLevel);

            helperContainer.SetActive(displayMode == SkillDisplayMode.Hotbar);

            displayPanelTransform.gameObject.SetActive(true);

            SetDisplayPosition(displayMode);
        }

        public void HideSkillInfo()
        {
            if (currentIconTransform == null | currentSkillNode == null) return;
            displayPanelTransform.gameObject.SetActive(false);
            for (int i = 0; i < currentSkillNode.prerequisites.Count; i++)
            {
                requirementsPool[i].gameObject.SetActive(false);
            }
            requirementsBlock.SetActive(false);
            currentIconTransform = null;
            currentSkillNode = null;
        }

        public void SkillTree_SetDisplayPosition()
        {
            SetDisplayPosition(SkillDisplayMode.SkillTree);
        }

        private void SetDisplayPosition(SkillDisplayMode displayMode)
        {
            if(currentIconTransform == null | currentSkillNode == null) return;

            Vector2 iconPosition = currentIconTransform.position;
            bool iconOnRight = iconPosition.x >= CanvasSize.x / 2f;
            float horizModifier = iconOnRight || displayMode == SkillDisplayMode.Hotbar ? -1 : 1;

            iconPosition += new Vector2(horizModifier * currentIconTransform.rect.width / 2f, currentIconTransform.rect.height / 2f) * canvas.scaleFactor;

            Vector2 offset = new(horizModifier, 1f);
            AnchorPresets anchorPreset;
            switch (displayMode)
            {
                case SkillDisplayMode.SkillTree:
                    offset.Scale(treeDisplayOffset);
                    anchorPreset = iconOnRight ? AnchorPresets.TopRight : AnchorPresets.TopLeft;
                    break;
                case SkillDisplayMode.SkillButton:
                    offset.Scale(skillButtonOffset);
                    anchorPreset = AnchorPresets.BottomLeft;
                    break;
                case SkillDisplayMode.Hotbar:
                    offset.Scale(hotbarDisplayOffset);
                    anchorPreset = AnchorPresets.BottomLeft;
                    break;
                default:
                    anchorPreset = AnchorPresets.BottomLeft;
                    break;
            }

            Vector2 displayPosition = iconPosition + offset * canvas.scaleFactor;
            displayPanelTransform.position = displayPosition;

            // if icon on right then display on left and anchor is topright
            displayPanelTransform.SetAnchor(anchorPreset, AnchorPositionMode.SetCornerPosition);
            displayPanelTransform.SetAnchor(AnchorPresets.MiddleCenter, AnchorPositionMode.RetainPosition);

            // Check if offscreen
            Vector3[] corners = new Vector3[4];
            displayPanelTransform.GetWorldCorners(corners);
            var bottomLeft = corners[0];
            var topRight = corners[2];

            var newBottom = bottomLeft;
            var newTop = topRight;

            newBottom.y = newBottom.y <= 0 ? 0 : newBottom.y;
            newBottom.x = newBottom.x <= 0 ? 0 : newBottom.x;

            if(newBottom != bottomLeft)
            {
                displayPanelTransform.position = newBottom;
                displayPanelTransform.SetAnchor(AnchorPresets.BottomLeft, AnchorPositionMode.SetCornerPosition);
            }

            newTop.y = newTop.y >= CanvasSize.y ? CanvasSize.y : newTop.y;
            newTop.x = newTop.x >= CanvasSize.x ? CanvasSize.x : newTop.x;

            if (newTop != topRight)
            {
                displayPanelTransform.position = newTop;
                displayPanelTransform.SetAnchor(AnchorPresets.TopRight, AnchorPositionMode.SetCornerPosition);
            }

            displayPanelTransform.SetAnchor(AnchorPresets.MiddleCenter, AnchorPositionMode.RetainPosition);
        }
    }
}
