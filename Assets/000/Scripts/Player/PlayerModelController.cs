using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerModelController : PlayerControllerComponent
    {
        [SerializeField] private Material localPlayerMaterial;
        [SerializeField] private Material remotePlayerMaterial;

        [SerializeField] private Renderer render;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner)
            {
                render.material = remotePlayerMaterial;
                return;
            }
            else
            {
                render.material = localPlayerMaterial;
            }
        }
    }
}
