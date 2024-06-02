using Magus.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Magus.MatchmakingSystem
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager instance;

        public class LobbyEventArgs : EventArgs
        {
            public Lobby lobby;
        }

        public event EventHandler<LobbyEventArgs> OnJoinedLobby;
        public event EventHandler<LobbyEventArgs> OnLobbyUpdate;
        public event EventHandler<LobbyEventArgs> OnLobbyKicked;
        public event EventHandler OnLobbyDeleted;
        public event EventHandler OnLeftLobby;
        public event EventHandler OnGameStarted;


        private Lobby _lobby;
        public Lobby Lobby => _lobby;

        private string _lobbyCode;
        public string LobbyCode => _lobbyCode;

        private string _playerId;
        public string PlayerId => _playerId;

        public Player LocalPlayer => _lobby.Players.Find(x => x.Id == _playerId);

        private bool _isHost;
        public bool IsHost => _isHost;

        private ILobbyEvents lobbyEvents;

        private float heartbeatTimer;
        private float lobbyUpdateTimer;

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

        private void Update()
        {
            HandleHeartbeat();
            // PollLobbyUpdate();
        }

        private async void HandleHeartbeat()
        {
            if(_lobby != null && _isHost)
            {
                if(heartbeatTimer > 0)
                {
                    heartbeatTimer -= Time.deltaTime;
                }
                else
                {
                    heartbeatTimer = Constants.LOBBY_HEARTBEAT_TIME;
                    await LobbyService.Instance.SendHeartbeatPingAsync(_lobby.Id);
                }
            }
        }

        private async void PollLobbyUpdate()
        {
            if(_lobby != null)
            {
                if(lobbyUpdateTimer > 0)
                {
                    lobbyUpdateTimer -= Time.deltaTime;
                }
                else
                {
                    lobbyUpdateTimer = Constants.LOBBY_UPDATE_RATE;

                    Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
                    _lobby = updatedLobby;

                    OnLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = updatedLobby });

                    if (!IsPlayerInLobby())
                    {
                        OnLobbyKicked?.Invoke(this, new LobbyEventArgs { lobby = _lobby });
                        _lobby = null;
                        return;
                    }

                    if (!_isHost && _lobby.Data["RelayCode"].Value != "0")
                    {
                        Player player = _lobby.Players.Find(x => x.Id == _playerId);
                        if(player != null)
                        {
                            if (!bool.Parse(player.Data["InGame"].Value))
                            {
                                await RelayManager.instance.JoinRelay(_lobby.Data["RelayCode"].Value);
                                OnGameStarted?.Invoke(this, EventArgs.Empty);
                            }
                        }
                        
                    }
                }
            }
        }

        public async Task<LobbyServiceException> CreateLobbyInstance(string lobbyName, bool isPrivate)
        {
            try
            {
                Player player = GetPlayer();
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Player = player,
                    Data = new Dictionary<string, DataObject>
                    {
                        { "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, "0") },
                    }
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, Constants.MAX_PLAYERS, lobbyOptions);

                _lobby = lobby;
                _lobbyCode = lobby.LobbyCode;
                _playerId = player.Id;
                _isHost = true;

                await SubscribeToCallbacks();

                await SetPlayerNumber("1");

                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _lobby });

                return null;
            }
            catch (LobbyServiceException e)
            {
                return e;
            }
        }

        public async Task<LobbyServiceException> JoinLobbyInstance(string joinCode)
        {
            try
            {
                Player player = GetPlayer();
                JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
                {
                    Player = player,
                };

                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode, joinOptions);
                _lobby = lobby;
                _lobbyCode = lobby.LobbyCode;
                _playerId = player.Id;
                _isHost = false;

                await SubscribeToCallbacks();

                await SetJoinPlayerNumber();

                OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _lobby });

                return null;
            }
            catch (LobbyServiceException e)
            {
                return e;
            }
            catch (ArgumentNullException)
            {
                return new LobbyServiceException(LobbyExceptionReason.InvalidJoinCode, "Empty Join Code");
            }
            catch
            {
                return new LobbyServiceException(LobbyExceptionReason.Unknown, "Unknown Error");
            }
        }

        private async Task SubscribeToCallbacks()
        {
            var callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;
            callbacks.KickedFromLobby += OnKickedFromLobby;
            callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerDataChanged += PlayerDataChanged;
            callbacks.PlayerLeft += OnPlayerLeft;

            try
            {
                lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(_lobby.Id, callbacks);
            }
            catch (LobbyServiceException ex)
            {
                switch (ex.Reason)
                {
                    case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{_lobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                    case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                    case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                }
            }
        }

        private void PlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> dictionary)
        {
            foreach (var item in dictionary)
            {
                print("Key: " + item.Key);
                foreach (var value in dictionary[item.Key])
                {
                    print("Value: " + value.Value.Value.Value);
                }
            }
        }

        private async void OnLobbyChanged(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
                OnLobbyDeleted?.Invoke(this, EventArgs.Empty);
                ResetLobbyInfo();
            }
            else if(IsPlayerInLobby())
            {
                print("Update");
                changes.ApplyToLobby(_lobby);

                // print($"Joined: {changes.PlayerJoined.Changed} Left: {changes.PlayerLeft.Changed}");

                OnLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _lobby });

                if (!_isHost && _lobby.Data["RelayCode"].Value != "0")
                {
                    Player player = _lobby.Players.Find(x => x.Id == _playerId);
                    if (player != null)
                    {
                        if (!bool.Parse(player.Data["InGame"].Value))
                        {
                            await RelayManager.instance.JoinRelay(_lobby.Data["RelayCode"].Value);
                            OnGameStarted?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }

                Player hostPlayer = _lobby.Players.Find(x => x.Id == _lobby.HostId);
                if(!_isHost && hostPlayer != null && hostPlayer.Data["PlayerNumber"].Value == LocalPlayer.Data["PlayerNumber"].Value)
                {
                    await TogglePlayerNumber();
                }
            }
        }

        private async void OnPlayerJoined(List<LobbyPlayerJoined> playersJoined)
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
            _lobby = lobby;
            OnLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _lobby });
        }

        private async void OnPlayerLeft(List<int> list)
        {
            OnLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _lobby });
            print("PlayerLeft");
            await UpdateHost();
        }

        private void OnKickedFromLobby()
        {
            print("Kicked");
            OnLobbyKicked?.Invoke(this, new LobbyEventArgs { lobby = _lobby });
            ResetLobbyInfo();
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState connectionState)
        {
            // print(connectionState.ToString());
            switch (connectionState)
            {
                case LobbyEventConnectionState.Unknown:
                    break;
                case LobbyEventConnectionState.Unsubscribed:
                    break;
                case LobbyEventConnectionState.Subscribing:
                    break;
                case LobbyEventConnectionState.Subscribed:
                    break;
                case LobbyEventConnectionState.Unsynced:
                    break;
                case LobbyEventConnectionState.Error:
                    OnLeftLobby?.Invoke(this, EventArgs.Empty);
                    ResetLobbyInfo();
                    break;
            }
        }

        private bool IsPlayerInLobby()
        {
            if(_lobby != null && _lobby.Players != null)
            {
                foreach (Player player in _lobby.Players)
                {
                    if(player.Id == _playerId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Player GetPlayer()
        {
            return new Player(AuthenticationService.Instance.PlayerId)
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerInfoManager.instance.PlayerInfo.username)},
                    { "ReadyCheck", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, false.ToString()) },
                    { "InGame", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, false.ToString()) },
                    { "PlayerNumber", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "") }
                }
            };
        }

        public async void UpdateReadyCheck(bool isReady)
        {
            try
            {
                UpdatePlayerOptions playerOptions = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "ReadyCheck", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady.ToString()) }
                    }
                };
                Lobby newLobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, _playerId, playerOptions);
                _lobby = newLobby;

            }
            catch (LobbyServiceException)
            {

            }
        }

        public async void UpdateGameStatus(bool inGame)
        {
            try
            {
                UpdatePlayerOptions playerOptions = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "InGame", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, inGame.ToString()) }
                    }
                };
                Lobby newLobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, _playerId, playerOptions);
                _lobby = newLobby;

            }
            catch (LobbyServiceException)
            {

            }
        }

        public async Task TogglePlayerNumber()
        {
            if(_lobby != null)
            {
                string currentNumber = LocalPlayer.Data["PlayerNumber"].Value;
                await SetPlayerNumber(currentNumber == "1" ? "2" : "1");
            }
        }

        private async Task SetPlayerNumber(string playerNumber)
        {
            try
            {
                UpdatePlayerOptions playerOptions = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerNumber", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerNumber) }
                    }
                };
                Lobby newLobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, _playerId, playerOptions);
                //_lobby = newLobby;

            }
            catch (LobbyServiceException)
            {

            }
        }

        private async Task SetJoinPlayerNumber()
        {
            if(_lobby != null)
            {
                bool playerOne = false, playerTwo = false;
                foreach (var player in _lobby.Players)
                {
                    if (player.Data["PlayerNumber"].Value == "1") playerOne = true;
                    else if (player.Data["PlayerNumber"].Value == "2") playerTwo = true;
                }
                
                if(playerOne && playerTwo)
                {
                    // Spectator
                }
                else if (!playerOne && !playerTwo)
                {
                    await SetPlayerNumber("1");
                }
                else
                {
                    await SetPlayerNumber((playerTwo) ?  "1" : "2");
                }
            }
        }

        public async void SetAllocationId(string allocationId)
        {
            try
            {
                UpdatePlayerOptions playerOptions = new UpdatePlayerOptions
                {
                    AllocationId = allocationId
                };
                Lobby newLobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, _playerId, playerOptions);
                //_lobby = newLobby;
            }
            catch (LobbyServiceException)
            {

            }
        }

        public async Task LeaveLobby()
        {
            try
            {
                if( _lobby != null )
                {
                    await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, _playerId);
                    ResetLobbyInfo();
                }
            }
            catch (LobbyServiceException)
            {

            }
        }

        public async void KickPlayer(string playerId)
        {
            try
            {
                if(_isHost)
                {
                    await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, playerId);
                }
            }
            catch (LobbyServiceException)
            {

            }
        }

        public async void ChangeHost(string playerId)
        {
            try
            {
                if (_isHost)
                {
                    Lobby updatedLobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, new UpdateLobbyOptions
                    {
                        HostId = playerId,
                    });
                    _lobby = updatedLobby;
                    _isHost = false;
                }
            }
            catch (LobbyServiceException)
            {

            }
        }

        private async Task UpdateHost()
        {
            try
            {
                Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
                _lobby = updatedLobby;
                if (_playerId == _lobby.HostId)
                {
                    _isHost = true;
                    OnLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _lobby });
                }
            }
            catch (LobbyServiceException)
            {

            }
            
        }

        public async Task StartGame()
        {
            if(_isHost && _lobby.Players.Count == Constants.MAX_PLAYERS)
            {
                try
                {
                    string relayCode = await RelayManager.instance.CreateRelay();

                    UpdateLobbyOptions options = new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            { "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                        }
                    };

                    Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);

                    _lobby = lobby;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                }
                catch (LobbyServiceException)
                {

                }
            }
        }

        public async Task ReconnectToLobby()
        {
            try
            {
                await LobbyService.Instance.ReconnectToLobbyAsync(_lobby.Id);
            }
            catch
            {

            }
        }

        private async void ResetLobbyInfo()
        {
            if (lobbyEvents != null)
            {
                await lobbyEvents.UnsubscribeAsync();
            }
            lobbyEvents = null;
            _lobby = null;
            _isHost = false;
            _lobbyCode = "";
            _playerId = "";
        }

        private async void OnApplicationQuit()
        {
            if(_lobby != null) await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, _playerId);
        }
    }
}
