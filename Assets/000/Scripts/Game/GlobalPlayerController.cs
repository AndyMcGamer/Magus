using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Game
{
    public class GlobalPlayerController : NetworkBehaviour
    {
        public SkillDatabase skillDatabase;

        private readonly SyncVar<float> health_PlayerOne = new(new SyncTypeSettings(1f));
        private readonly SyncVar<float> health_PlayerTwo = new(new SyncTypeSettings(1f));
    }
}
