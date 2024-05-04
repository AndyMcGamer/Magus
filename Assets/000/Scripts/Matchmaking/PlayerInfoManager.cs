using Magus.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.MatchmakingSystem
{
    public class PlayerInfoManager : MonoBehaviour, IDataPersistence
    {
        public static PlayerInfoManager instance;

        public event EventHandler<string> OnUsernameUpdated;

        private PlayerInfo _playerInfo;
        public PlayerInfo PlayerInfo => _playerInfo;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void UpdateUsername(string newUsername)
        {
            _playerInfo.username = newUsername;
            OnUsernameUpdated?.Invoke(this, newUsername);
        }

        public void LoadData(SaveData data)
        {
            _playerInfo = data.playerInfo;
            OnUsernameUpdated?.Invoke(this, PlayerInfo.username);
        }

        public void SaveData(ref SaveData data)
        {
            data.playerInfo = _playerInfo;
        }
    }
}
