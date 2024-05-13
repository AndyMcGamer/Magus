using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magus.SceneSpecific
{
    public class GameLoader : NetworkBehaviour
    {
        private readonly SyncVar<float> countdownTimer = new SyncVar<float>(new SyncTypeSettings(1f));
        [SerializeField] private float initialCountdownTime;
        [SerializeField] private float acceleratedCountdownTime;

        [SerializeField] private int minPlayers;
        [SerializeField] private int neededTrainingRooms;

        public event Action<float> OnCountdownChanged;

        private bool activatedSceneChange;

        private void Awake()
        {
            countdownTimer.OnChange += CountdownChanged;
            OnCountdownChanged?.Invoke(initialCountdownTime);
            if (base.IsServerInitialized)
            {
                base.SceneManager.OnClientLoadedStartScenes += ClientLoadedStartScene;
                base.SceneManager.OnLoadEnd += OnLoadEnd;
            }
        }

        private void OnDestroy()
        {
            countdownTimer.OnChange -= CountdownChanged;
            if (base.IsServerInitialized)
            {
                base.SceneManager.OnClientLoadedStartScenes -= ClientLoadedStartScene;
                base.SceneManager.OnLoadEnd -= OnLoadEnd;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            countdownTimer.Value = initialCountdownTime;
            activatedSceneChange = false;
            print("Server Start");
            base.SceneManager.OnClientLoadedStartScenes += ClientLoadedStartScene;
            base.SceneManager.OnLoadEnd += OnLoadEnd;
        }

        private void ClientLoadedStartScene(NetworkConnection connection, bool isServer)
        {
            print("Client Load Start");
            if (isServer)
            {
                base.SceneManager.AddConnectionToScene(connection, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        private void OnLoadEnd(SceneLoadEndEventArgs args)
        {
            if (!args.QueueData.AsServer) return;
            Scene loadedScene = args.LoadedScenes.FirstOrDefault(x => x.name == "LoadingScene");
            if (loadedScene.name != null)
            {
                // Expect only integers for loading scene
                int[] serverParams = args.QueueData.SceneLoadData.Params.ServerParams.Cast<int>().ToArray();
                minPlayers = serverParams[0];
                neededTrainingRooms = minPlayers;
                return;
            }
            loadedScene = args.LoadedScenes.FirstOrDefault(x => x.name == "TrainingRoom");
            if(loadedScene.name != null)
            {
                neededTrainingRooms--;
            }
            if(neededTrainingRooms == 0)
            {
                SceneSwitcher.instance.UnloadGlobalNetworkedScene("LoadingScene");
                List<Scene> openScenes = new();
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                {
                    Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                    if (scene.name == "TrainingRoom")
                    {
                        openScenes.Add(scene);
                    }
                }
                int j = 0;
                foreach (var conn in ServerManager.Clients.Values)
                {
                    SceneSwitcher.instance.LoadStackedNetworkScene(openScenes[j].handle, conn);
                    ++j;
                }
                //SceneSwitcher.instance.UnloadGlobalNetworkedScene("LoadingScene");
            }
        }

        private void CountdownChanged(float prev, float next, bool asServer)
        {
            if(!asServer)
            {
                OnCountdownChanged?.Invoke(next);
            }
            else
            {
                if(next <= 0 && !activatedSceneChange)
                {
                    SceneSwitcher.instance.PreloadNetworkScenes("TrainingRoom");
                    activatedSceneChange = true;
                    //SceneSwitcher.instance.UnloadGlobalNetworkedScene("LoadingScene");
                    //SceneSwitcher.instance.LoadStackedNetworkScene("TrainingRoom", ServerManager.Clients.Values.ToArray());
                }
            }
        }

        private void Update()
        {
            if (base.IsServerInitialized)
            {
                if(countdownTimer.Value > 0)
                {
                    countdownTimer.Value -= Time.deltaTime;
                    if(minPlayers == base.ServerManager.Clients.Count && countdownTimer.Value > acceleratedCountdownTime)
                    {
                        countdownTimer.Value = acceleratedCountdownTime;
                    }
                }
            }
        }
    }
}
