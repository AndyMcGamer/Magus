using FishNet.Object;
using Magus.Skills;
using Magus.Game;
using Magus.Global;
using Magus.Multiplayer;
using Magus.Skills.ActiveSkills;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magus.PlayerController
{
    public class PlayerAttack : PlayerControllerComponent
    {
        public void CastProjectileSkill(ProjectileSkillData skillData)
        {
            Vector3 position = transform.position + skillData.spawnOffset;
            Vector3 direction = playerInfo.lastMove;

            int playerNumber = ConnectionManager.instance.playerData[base.LocalConnection];

            if (!base.IsServerStarted) SpawnProjectile(playerNumber, skillData, position, direction, 0f);
            FireProjectileServer(skillData.Name, position, direction, base.TimeManager.Tick, playerNumber);
        }

        private void SpawnProjectile(int playerNumber, ProjectileSkillData skillData, Vector3 position, Vector3 direction, float timePassed)
        {
            var spawnedProjectile = Instantiate(skillData.projectilePrefab, position, Quaternion.LookRotation(direction)).GetComponent<Projectile>();
            spawnedProjectile.Initialize(skillData, timePassed, playerNumber);
        }

        private void SpawnProjectile(int playerNumber, ProjectileSkillData skillData, Vector3 position, Vector3 direction, float timePassed, Scene scene)
        {
            var spawnedProjectile = Instantiate(skillData.projectilePrefab, position, Quaternion.LookRotation(direction)).GetComponent<Projectile>();
            spawnedProjectile.Initialize(skillData, timePassed, playerNumber, showVisual: scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(spawnedProjectile.gameObject, scene);
        }

        [ServerRpc]
        private void FireProjectileServer(string skillName, Vector3 position, Vector3 direction, uint tick, int playerNumber)
        {
            print(playerNumber);
            float passedTime = (float)base.TimeManager.TimePassed(tick, false);

            passedTime = Mathf.Min(Constants.PROJECTILE_MAX_PASSED_TIME / 2f, passedTime);

            ProjectileSkillData skillData = GlobalPlayerController.instance.skillDatabase.FindDataByName<ProjectileSkillData>(skillName);

            switch (RoundController.instance.gameStage)
            {
                case GameStage.Training:
                    SpawnProjectile(playerNumber, skillData, position, direction, passedTime, GlobalPlayerController.instance.trainingRooms[playerNumber]);
                    break;
                case GameStage.Battle:
                case GameStage.SuddenDeath:
                    SpawnProjectile(playerNumber, skillData, position, direction, passedTime);
                    break;
            }

            
            FireProjectileObservers(skillName, position, direction, tick, playerNumber);
        }

        [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
        private void FireProjectileObservers(string skillName, Vector3 position, Vector3 direction, uint tick, int playerNumber)
        {
            float passedTime = (float)base.TimeManager.TimePassed(tick, false);
            passedTime = Mathf.Min(Constants.PROJECTILE_MAX_PASSED_TIME, passedTime);

            ProjectileSkillData skillData = GlobalPlayerController.instance.skillDatabase.FindDataByName<ProjectileSkillData>(skillName);
            SpawnProjectile(playerNumber, skillData, position, direction, passedTime);
        }
    }
}
