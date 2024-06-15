using DG.Tweening;
using Magus.MatchmakingSystem;
using Magus.SaveSystem;
using Magus.SceneManagement;
using Magus.UserInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    [System.Serializable]
    public class MainMenuScreen
    {
        public string name;
        public GameObject screen;
    }

    public class MainMenu : MonoBehaviour, IDataPersistence
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

        [Header("Controls")]
        [SerializeField] private Image controlsBackground;
        [SerializeField] private Transform controlsPanel;
        private bool showControls;

        private bool shownControlsBefore;

        private void Awake()
        {
            GoToScreen("Title");
            showControls = false;
            controlsBackground.DOFade(0f, 0.01f);
            controlsBackground.raycastTarget = false;
            controlsPanel.localScale = Vector3.zero;
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

        public void ToggleControls()
        {
            showControls = !showControls;
            if (showControls)
            {
                controlsBackground.DOFade(1f, 0.35f).SetEase(Ease.OutSine);
                controlsBackground.raycastTarget = true;
                controlsPanel.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutSine);
            }
            else
            {
                controlsBackground.DOFade(0f, 0.35f).SetEase(Ease.OutSine);
                controlsBackground.raycastTarget = false;
                controlsPanel.DOScale(Vector3.zero, 0.35f).SetEase(Ease.OutSine);
            }
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

        public async void GoToScreen(string screenName)
        {
            await Fader.instance.FadeIn(0.55f, DG.Tweening.Ease.InOutSine, false);
            DisableAllScreens();
            lobbyMenuUI.ResetScreens();
            var screen = System.Array.Find(mainMenuScreens, x => x.name == screenName);
            screen.screen.SetActive(true);
            CancelEditUsername();
            await Fader.instance.FadeOut(0.55f, DG.Tweening.Ease.InOutSine, false);
        }

        private void GoToScene(string sceneName)
        {
            SceneSwitcher.instance.LoadScene(sceneName);
        }

        public async void GotoCredits()
        {
            await Fader.instance.FadeIn(0.5f, DG.Tweening.Ease.OutSine);
            SceneSwitcher.instance.LoadScene("Credits");
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

        public async void StartTraining()
        {
            await Fader.instance.FadeIn(0.65f, DG.Tweening.Ease.InOutSine);
            trainingStarter.StartTrainingRoom();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void LoadData(SaveData data)
        {
            shownControlsBefore = data.shownControls;
            if (!shownControlsBefore)
            {
                ToggleControls();
                shownControlsBefore = true;
            }
        }

        public void SaveData(ref SaveData data)
        {
            data.shownControls = shownControlsBefore;
        }
    }
}
