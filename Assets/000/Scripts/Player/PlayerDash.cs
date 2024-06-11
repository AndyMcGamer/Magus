using DG.Tweening;
using FishNet.Object;
using Magus.Global;
using Magus.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerDash : PlayerControllerComponent
    {
        private Vector3 targetPosition;
        private Tweener dashTween;

        public void CastDashSkill(DashSkillData dashSkill, int skillLevel)
        {
            Physics.IgnoreLayerCollision(Constants.PLAYER_ONE_LAYER, Constants.PLAYER_TWO_LAYER, true);

            dashTween.Kill();
            dashTween = null;

            Vector3 dashDirectionWorld = playerInfo.playerModel.TransformDirection(dashSkill.dashDirection).normalized;

            float dashTime = dashSkill.dashDuration[skillLevel];
            float dashDistance = dashSkill.dashDistance[skillLevel];

            Vector3 endPosition = transform.position + dashDirectionWorld * dashDistance;
            targetPosition = transform.position;

            dashTween = DOTween.To(() => targetPosition, x => targetPosition = x, endPosition, dashTime).SetEase(dashSkill.easeFunction).OnKill(() => { dashTween = null; Physics.IgnoreLayerCollision(Constants.PLAYER_ONE_LAYER, Constants.PLAYER_TWO_LAYER, false); });
        }

        private void Update()
        {
            if(dashTween != null)
            {
                //print(targetPosition);
                playerInfo.characterController.Move(targetPosition - transform.position);
            }
        }
    }
}
