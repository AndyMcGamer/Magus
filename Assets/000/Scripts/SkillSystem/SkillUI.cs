using System.Collections;
using System.Collections.Generic;
using Magus.Game;
using Magus.PlayerController;
using Magus.Skills.SkillTree;
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

        [Header("Settings")]
        [SerializeField] private Color regularBorderColor;
        [SerializeField] private Color activatedBorderColor;

        private void Awake()
        {
            ToggleBorder(false);
            playerInfo = GetComponentInParent<PlayerControllerInfo>();
        }

        private void OnEnable()
        {
            playerInfo.skillManager.OnSkillUpdate += SkillManager_OnSkillUpdate;
        }

        private void OnDisable()
        {
            playerInfo.skillManager.OnSkillUpdate -= SkillManager_OnSkillUpdate;
        }

        private void SkillManager_OnSkillUpdate(int playerNumber)
        {
            CheckPrerequisites(playerNumber);
        }

        private bool CheckPrerequisites(int playerNumber)
        {
            bool passed = true;
            foreach (var prereq in skillNode.prerequisites)
            {
                string skillName = prereq.skill.Name;
                int skillLevel = GlobalPlayerController.instance.GetSkillStatus(playerNumber)[skillName];
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
