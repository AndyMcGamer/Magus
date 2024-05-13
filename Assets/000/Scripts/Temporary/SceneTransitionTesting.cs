using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;

namespace Magus.Temporary
{
    public class SceneTransitionTesting : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                NetworkConnection localConnection = InstanceFinder.ClientManager.Connection;
                SceneLoadData sld = new SceneLoadData("LoadingScene");
                sld.Options.AllowStacking = true;
                sld.ReplaceScenes = ReplaceOption.All;
                sld.Options.AutomaticallyUnload = true;
                sld.Options.LocalPhysics = UnityEngine.SceneManagement.LocalPhysicsMode.Physics3D;
                sld.PreferredActiveScene = new PreferredScene(new SceneLookupData("LoadingScene"));

                InstanceFinder.SceneManager.LoadConnectionScenes(localConnection, sld);
            }
        }
    }
}
