using FishNet.Object;
using Magus.ActiveSkills;
using Magus.Global;
using Magus.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerAttack : PlayerControllerComponent
    {
         
        public void CastProjectileSkill(ProjectileSkillData skillData)
        {
            Vector3 position = transform.position;
            Vector3 direction = playerInfo.lastMove;

            SpawnProjectile(skillData, position, direction, 0f);
            FireProjectileServer(skillData, position, direction, base.TimeManager.Tick);
        }

        private void SpawnProjectile(ProjectileSkillData skillData, Vector3 position, Vector3 direction, float timePassed)
        {
            var spawnedProjectile = Instantiate(skillData.projectilePrefab, position, Quaternion.LookRotation(direction)).GetComponent<Magus.ActiveSkills.Projectile>();
            spawnedProjectile.Initialize(skillData, direction, timePassed, playerInfo.playerTag);
        }

        [ServerRpc]
        private void FireProjectileServer(ProjectileSkillData skillData, Vector3 position, Vector3 direction, uint tick)
        {
            float passedTime = (float)base.TimeManager.TimePassed(tick, false);

            passedTime = Mathf.Min(Constants.PROJECTILE_MAX_PASSED_TIME / 2f, passedTime);

            SpawnProjectile(skillData, position, direction, passedTime);
            FireProjectileObservers(skillData, position, direction, tick);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void FireProjectileObservers(ProjectileSkillData skillData, Vector3 position, Vector3 direction, uint tick)
        {
            float passedTime = (float)base.TimeManager.TimePassed(tick, false);
            passedTime = Mathf.Min(Constants.PROJECTILE_MAX_PASSED_TIME, passedTime);

            SpawnProjectile(skillData, position, direction, passedTime);
        }
    }
}
