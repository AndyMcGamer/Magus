using FishNet.Managing.Scened;
using FishNet.Object;
using Magus.Global;
using Magus.Multiplayer;
using Magus.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magus.Game
{
    public enum GameMode
    {
        Training,
        Standard
    }

    public class MatchController : NetworkBehaviour
    {
        public static MatchController instance;

        public int roundNumber;
        public GameMode gameMode;

        private int minPlayers;
        public int MinPlayers => minPlayers;
        private int neededTrainingRooms;

        public int wins_PlayerOne;
        public int wins_PlayerTwo;

        private bool matchStarted = false;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void OnDestroy()
        {
            if (base.IsServerInitialized) base.SceneManager.OnLoadEnd -= OnLoadEnd;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            base.SceneManager.OnLoadEnd += OnLoadEnd;
        }

        /// <summary>
        /// If all training rooms loaded, then the round should be restarting
        /// </summary>
        /// <param name="args"></param>
        private void OnLoadEnd(SceneLoadEndEventArgs args)
        {
            if (!args.QueueData.AsServer || !matchStarted) return;

            Scene loadedScene = args.LoadedScenes.FirstOrDefault(x => x.name == "TrainingRoom");
            if (loadedScene.name != null)
            {
                neededTrainingRooms--;
                if (neededTrainingRooms == 0)
                {
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
                    RoundController.instance.StartRound();
                }
            }
        }

        public void SetMode(GameMode mode)
        {
            gameMode = mode;
            SetGameMode(mode);
        }

        public void StartMatch(GameMode mode, int minPlayers)
        {
            Client_StartMatch();
            matchStarted = true;
            gameMode = mode;
            SetGameMode(mode);
            this.minPlayers = minPlayers;
            SetMinPlayers(minPlayers);
            roundNumber = 1;
            RoundController.instance.StartRound();
            SetRoundNumber(roundNumber);
            ResetPlayerWins();

            switch (gameMode)
            {
                case GameMode.Training:
                    GlobalPlayerController.instance.SetSkillPoints(1, 999);
                    break;
                case GameMode.Standard:
                    GlobalPlayerController.instance.SetSkillPoints(1, 5);
                    GlobalPlayerController.instance.SetSkillPoints(2, 5);
                    break;
            }
        }

        [ObserversRpc]
        private void Client_StartMatch()
        {
            AudioManager.instance.StopAllAudio();
            AudioManager.instance.Play("Battle");
        }

        [ObserversRpc(ExcludeServer = false)]
        private void SetMinPlayers(int players)
        {
            minPlayers = players;
        }

        [Server]
        public void EndRound(int winningPlayer)
        {
            print(winningPlayer);
            // if number of wins met, end game
            if(wins_PlayerOne == Constants.NUM_WINS_NEEDED || wins_PlayerTwo == Constants.NUM_WINS_NEEDED)
            {
                // End Game
                EndGame(wins_PlayerOne == Constants.NUM_WINS_NEEDED ? 1 : 2);
                return;
            }

            if (winningPlayer != 0)
            {
                roundNumber++;
                SetRoundNumber(roundNumber);
                GlobalPlayerController.instance.ChangeSkillPoints(1, 3);
                GlobalPlayerController.instance.ChangeSkillPoints(2, 3);

                GlobalPlayerController.instance.ChangeSkillPoints(winningPlayer, 1);
            }

            GlobalPlayerController.instance.ClearTrainingRooms();
            neededTrainingRooms = minPlayers;
            SceneSwitcher.instance.LoadGlobalNetworkedScene("RoundTimer", false, ReplaceOption.All);
            SceneSwitcher.instance.PreloadNetworkScenes("TrainingRoom");
        }

        private void EndGame(int winner)
        {
            LoadParams lp = new() { ServerParams = new object[] { winner } };
            SceneSwitcher.instance.LoadGlobalNetworkedScene("EndScene", lp);
        }

        private void ResetPlayerWins()
        {
            wins_PlayerOne = 0;
            wins_PlayerTwo = 0;
            SetPlayerWins(1, wins_PlayerOne);
            SetPlayerWins(2, wins_PlayerTwo);
        }

        [Server]
        public void ChooseWinner(int winnerNum)
        {
            IncrementPlayerWins(winnerNum);
        }

        private void IncrementPlayerWins(int playerNumber)
        {
            if(playerNumber == 1)
            {
                wins_PlayerOne++;
                SetPlayerWins(1, wins_PlayerOne);
            }
            else if(playerNumber == 2)
            {
                wins_PlayerTwo++;
                SetPlayerWins(2, wins_PlayerTwo);
            }
        }

        [ObserversRpc(ExcludeServer = false)]
        private void SetPlayerWins(int playerNumber, int wins)
        {
            if(playerNumber == 1)
            {
                wins_PlayerOne = wins;
            }
            else if(playerNumber == 2)
            {
                wins_PlayerTwo = wins;
            }
        }


        [ObserversRpc(ExcludeServer = false)]
        private void SetRoundNumber(int roundNumber)
        {
            this.roundNumber = roundNumber;
        }

        [ObserversRpc(ExcludeServer = false)]
        private void SetGameMode(GameMode gameMode)
        {
            this.gameMode = gameMode;
        }
    }
}
