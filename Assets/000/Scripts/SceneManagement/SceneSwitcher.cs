using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magus.SceneManagement
{
    public class SceneSwitcher : MonoBehaviour
    {
        public static SceneSwitcher instance;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        public void LoadGlobalNetworkedScene(string sceneName, bool setPreferred = true, ReplaceOption replaceOption = ReplaceOption.All)
        {
            if (!InstanceFinder.IsServerStarted) return;
            SceneLoadData sld = new SceneLoadData(sceneName);
            sld.Options.AllowStacking = false;
            sld.ReplaceScenes = replaceOption;
            sld.Options.AutomaticallyUnload = true;
            if (setPreferred)
            {
                sld.PreferredActiveScene = new PreferredScene(new SceneLookupData(sceneName));
            }
            

            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        }

        public void LoadGlobalNetworkedScene(string sceneName, LoadParams loadParams)
        {
            if (!InstanceFinder.IsServerStarted) return;
            SceneLoadData sld = new SceneLoadData(sceneName);
            sld.Options.AllowStacking = false;
            sld.ReplaceScenes = ReplaceOption.All;
            sld.Options.AutomaticallyUnload = true;
            sld.PreferredActiveScene = new PreferredScene(new SceneLookupData(sceneName));

            sld.Params = loadParams;

            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        }

        public void LoadStackedNetworkScene(int sceneHandle, NetworkConnection conn)
        {
            if (!InstanceFinder.IsServerStarted) return;
            SceneLookupData lookup = new SceneLookupData(sceneHandle);
            SceneLoadData sld = new SceneLoadData(lookup);
            sld.Options.AllowStacking = true;
            sld.ReplaceScenes = ReplaceOption.All;
            sld.Options.LocalPhysics = LocalPhysicsMode.Physics3D;
            sld.PreferredActiveScene = new PreferredScene(new SceneLookupData("TrainingRoom"), lookup);
            sld.Options.AutomaticallyUnload = true;
            InstanceFinder.SceneManager.LoadConnectionScenes(conn, sld);
        }

        public void LoadStackedNetworkScene(string sceneName, NetworkConnection[] conns)
        {
            if (!InstanceFinder.IsServerStarted) return;
            SceneLookupData lookup = new SceneLookupData(sceneName);
            SceneLoadData sld = new SceneLoadData(lookup);
            sld.Options.AllowStacking = true;
            sld.ReplaceScenes = ReplaceOption.None;
            sld.Options.LocalPhysics = LocalPhysicsMode.Physics3D;
            sld.PreferredActiveScene = new PreferredScene(lookup);
            InstanceFinder.SceneManager.LoadConnectionScenes(conns, sld);
        }

        public void PreloadNetworkScenes(string sceneName)
        {
            if (!InstanceFinder.IsServerStarted) return;
            for (int i = 0; i < InstanceFinder.ServerManager.Clients.Count; i++)
            {
                SceneLoadData sld = new SceneLoadData(sceneName);
                sld.Options.AllowStacking = true;
                sld.ReplaceScenes = ReplaceOption.None;
                sld.Options.LocalPhysics = LocalPhysicsMode.Physics3D;

                InstanceFinder.SceneManager.LoadConnectionScenes(sld);
            }
        }

        public void UnloadGlobalNetworkedScene(string sceneName)
        {
            if (!InstanceFinder.IsServerStarted) return;
            SceneUnloadData sud = new SceneUnloadData(sceneName);
            InstanceFinder.SceneManager.UnloadGlobalScenes(sud);
        }
    }
}
