using Magus.MatchmakingSystem;
using Magus.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magus.SceneSpecific
{
    [System.Serializable]
    public class MainMenuScreen
    {
        public string name;
        public GameObject screen;
    }

    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private MainMenuScreen[] mainMenuScreens;

        [Header("References")]
        [SerializeField] private TrainingStarter trainingStarter;

        [Header("Lobby Menus")]
        [SerializeField] private LobbyMenuUI lobbyMenuUI;

        [Header("Username Display")]
        [SerializeField] private GameObject editUsernameHolder;
        [SerializeField] private GameObject usernameHolder;
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TMP_InputField editUsernameText;

        private void Awake()
        {
            GoToScreen("Title");
        }

        private void OnEnable()
        {
            PlayerInfoManager.instance.OnUsernameUpdated += UsernameChanged;
            LobbyManager.instance.OnJoinedLobby += JoinLobby;
        }

        private void OnDisable()
        {
            PlayerInfoManager.instance.OnUsernameUpdated -= UsernameChanged;
            LobbyManager.instance.OnJoinedLobby -= JoinLobby;
        }

        private void JoinLobby(object sender, LobbyManager.LobbyEventArgs e)
        {
            GoToScene("LobbyScene");
        }

        private void DisableAllScreens()
        {
            foreach (var screen in mainMenuScreens)
            {
                screen.screen.SetActive(false);
            }
        }

        public void GoToScreen(string screenName)
        {
            DisableAllScreens();
            lobbyMenuUI.ResetScreens();
            var screen = System.Array.Find(mainMenuScreens, x => x.name == screenName);
            screen.screen.SetActive(true);
            CancelEditUsername();
        }

        public void GoToScene(string sceneName)
        {
            SceneSwitcher.instance.LoadScene(sceneName);
        }

        public void StartEditUsername()
        {
            editUsernameText.text = PlayerInfoManager.instance.PlayerInfo.username;
            editUsernameHolder.SetActive(true);
            usernameHolder.SetActive(false);
        }

        public void ConfirmUsername()
        {
            PlayerInfoManager.instance.UpdateUsername(editUsernameText.text);
            CancelEditUsername();
        }

        private void CancelEditUsername()
        {
            editUsernameText.text = "";
            editUsernameHolder.SetActive(false);
            usernameHolder.SetActive(true);
        }

        private void UsernameChanged(object sender, string e)
        {
            usernameText.text = e;
        }

        public void StartTraining()
        {
            trainingStarter.StartTrainingRoom();
        }
    }
}
