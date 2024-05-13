using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.SceneManagement
{
    public class PhysicsSceneSync : NetworkBehaviour
    {
        private bool synchronizePhysics;

        private static HashSet<int> synchronizedScenes = new();

        public override void OnStartNetwork()
        {
            int sceneHandle = gameObject.scene.handle;

            if(synchronizedScenes.Contains(sceneHandle))
            {
                return;
            }

            synchronizePhysics = (gameObject.scene.GetPhysicsScene() != Physics.defaultPhysicsScene);

            if(synchronizePhysics)
            {
                synchronizedScenes.Add(sceneHandle);
                base.TimeManager.OnPrePhysicsSimulation += TimeManager_OnPrePhysicsSimulation;
            }

        }

        public override void OnStopNetwork()
        {
            if (synchronizePhysics)
            {
                synchronizedScenes.Remove(gameObject.scene.handle);
                base.TimeManager.OnPrePhysicsSimulation -= TimeManager_OnPrePhysicsSimulation;
            }
        }

        private void TimeManager_OnPrePhysicsSimulation(float delta)
        {
            if (synchronizePhysics) gameObject.scene.GetPhysicsScene().Simulate(delta);
        }
    }
}
