using System.Collections;
using System.Collections.Generic;
using Magus.Game;
using Magus.Multiplayer;
using Magus.PlayerController;
using Magus.Skills.SkillTree;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.Skills
{
    public class SkillUI : MonoBehaviour
    {
        public SkillNode skillNode;
        private PlayerControllerInfo playerInfo;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image spriteBorder;
        [SerializeField] private GameObject disableMask;
        [SerializeField, Foldout("Upgrade Buttons")] private Button addButton;
        [SerializeField, Foldout("Upgrade Buttons")] private Button subtractButton;

        [Header("Settings")]
        [SerializeField, Foldout("Border")] private Color regularBorderColor;
        [SerializeField, Foldout("Border")] private Color activatedBorderColor;
        [SerializeField, Foldout("Level Text")] private Color regularTextColor;
        [SerializeField, Foldout("Level Text")] private Color disabledTextColor;

        private bool unlocked;

        private void Awake()
        {
            unlocked = false;
            ToggleBorder(false);
            playerInfo = GetComponentInParent<PlayerControllerInfo>();
        }

        private void OnEnable()
        {
            playerInfo.skillManager.OnSkillUpdate += SkillManager_OnSkillUpdate;
            playerInfo.skillManager.PropogateSkillUpdate(skillNode.skillData.Name);
            GlobalPlayerController.instance.OnSkillPointsChanged += OnSkillPointsChanged;
        }

        private void OnDisable()
        {
            playerInfo.skillManager.OnSkillUpdate -= SkillManager_OnSkillUpdate;
            GlobalPlayerController.instance.OnSkillPointsChanged -= OnSkillPointsChanged;
        }

        private void OnSkillPointsChanged(int playerNumber, int prev, int next)
        {
            if (playerNumber != ConnectionManager.instance.playerData[playerInfo.LocalConnection]) return;
            if (prev != 0 && next != 0) return;
            UpdateSkillUI(playerNumber, skillNode.skillData.Name, false);
        }

        private void SkillManager_OnSkillUpdate(int playerNumber, string skillName)
        {   
            UpdateSkillUI(playerNumber, skillName);
        }

        private void UpdateSkillUI(int playerNumber, string skillName, bool prop = true)
        {
            if (skillName != skillNode.skillData.Name) return;

            var ownedSkills = GlobalPlayerController.instance.GetSkillStatus(playerNumber);

            ownedSkills.TryGetValue(skillName, out int skillLevel);

            levelText.text = $"{skillLevel}/{skillNode.skillData.MaxLevel}";


            bool previous = unlocked;
            unlocked = CheckPrerequisites(playerNumber);

            if (unlocked != previous && !unlocked)
            {
                GlobalPlayerController.instance.RemoveSkill(playerNumber, skillName);
            }

            ToggleLevelText(unlocked);
            ToggleUpgradeButtons(playerNumber, unlocked, skillLevel);
            ToggleDisableMask(unlocked);

            if (!prop) return;

            // Go down connected nodes
            foreach (var postReq in skillNode.postrequisites)
            {
                playerInfo.skillManager.PropogateSkillUpdate(postReq);
            }
        }

        private bool CheckPrerequisites(int playerNumber)
        {
            bool passed = true;
            foreach (var prereq in skillNode.prerequisites)
            {
                string skillName = prereq.skill.Name;
                var skillStatus = GlobalPlayerController.instance.GetSkillStatus(playerNumber);
                skillStatus.TryGetValue(skillName, out int skillLevel);
                if (skillLevel < prereq.requiredLevel)
                {
                    passed = false;
                    break;
                }
            }
            return passed;
        }

        public void UpdateSkillLevel(bool adding)
        {
            playerInfo.skillManager.UpdateSkill(skillNode.skillData.Name, adding);
        }

        public void ToggleBorder(bool activated)
        {
            spriteBorder.color = activated ? activatedBorderColor : regularBorderColor;
        }

        private void ToggleLevelText(bool activated)
        {
            levelText.color = activated ? regularTextColor : disabledTextColor;
        }

        private void ToggleUpgradeButtons(int playerNumber, bool activated, int level)
        {
            bool inTraining = RoundController.instance.gameStage == GameStage.Training;
            addButton.interactable = activated && level < skillNode.skillData.MaxLevel && GlobalPlayerController.instance.GetSkillPoints(playerNumber) > 0 && inTraining;
            subtractButton.interactable = activated && level > 0 && inTraining;
        }

        private void ToggleDisableMask(bool activated)
        {
            disableMask.SetActive(!activated);
        }

#if UNITY_EDITOR
        [SerializeField] private Image skillSprite;
        private void OnValidate()
        {
            if(skillNode != null)
            {
                skillSprite.sprite = skillNode.skillData.Icon;
            }
        }
#endif
    }
}
