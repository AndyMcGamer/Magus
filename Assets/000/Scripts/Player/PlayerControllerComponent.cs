using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public abstract class PlayerControllerComponent : NetworkBehaviour
    {
        protected PlayerControllerInfo playerInfo;
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner) { enabled = false; }
        }

        public void Init(PlayerControllerInfo playerInfoManager)
        {
            playerInfo = playerInfoManager;
        }
    }
}
