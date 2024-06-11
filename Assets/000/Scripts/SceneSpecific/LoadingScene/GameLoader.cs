using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Game;
using Magus.Global;
using Magus.Multiplayer;
using Magus.SceneManagement;
using Magus.UserInterface;
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

        [SerializeField] private int minPlayers;
        [SerializeField] private int neededTrainingRooms;

        public event Action<float> OnCountdownChanged;

        private bool activatedSceneChange;
        [SerializeField] private GameMode gameMode;

        private WaitForSeconds zeroPointFive = new(0.75f);

        private void Awake()
        {
            countdownTimer.OnChange += CountdownChanged;
            OnCountdownChanged?.Invoke(Constants.LOADING_INITIAL_TIME);

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
            countdownTimer.Value = Constants.LOADING_INITIAL_TIME;
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

                gameMode = (GameMode)serverParams[1];
                MatchController.instance.SetMode(gameMode);
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
                    GlobalPlayerController.instance.AddTrainingRoom(ConnectionManager.instance.playerData[conn], openScenes[j]);
                    ++j;
                }
                MatchController.instance.StartMatch(gameMode, minPlayers);
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
                    activatedSceneChange = true;

                    StartCoroutine(Fade());
                }
            }
        }

        private IEnumerator Fade()
        {
            GlobalPlayerController.instance.ClearTrainingRooms();
            SceneSwitcher.instance.LoadGlobalNetworkedScene("RoundTimer", false, ReplaceOption.None);
            
            Client_Fade(0.5f);
            yield return zeroPointFive;

            SceneSwitcher.instance.PreloadNetworkScenes("TrainingRoom");

        }

        [ObserversRpc]
        private void Client_Fade(float time)
        {
            Client_Fade_Async(time);
        }

        [Client]
        private async void Client_Fade_Async(float time)
        {
            await Fader.instance.FadeIn(time: time, easeFunction: DG.Tweening.Ease.InSine);
        }

        private void Update()
        {
            if (base.IsServerInitialized)
            {
                if(countdownTimer.Value > 0)
                {
                    countdownTimer.Value -= Time.deltaTime;
                    if(minPlayers == base.ServerManager.Clients.Count && countdownTimer.Value > Constants.LOADING_ACCEL_TIME)
                    {
                        countdownTimer.Value = Constants.LOADING_ACCEL_TIME;
                    }
                }
            }
        }
    }
}
