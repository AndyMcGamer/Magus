using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.PlayerController
{
    public class PlayerCameraController : PlayerControllerComponent
    {
        [SerializeField] private float initialDistance;
        [SerializeField] private float initialYOffset;
        [SerializeField] private float initialAngle;
        [SerializeField] private Vector2 cameraDistanceBounds;
        [SerializeField] private float minTargetOffset;
        [SerializeField] private float minCameraAngle;
        [SerializeField] private float scrollSpeed;
        
        private CinemachineFramingTransposer transposer;
        private float initialDistanceRatio;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner) return;
            this.enabled = true;
            playerInfo.playerCamera.enabled = true;
            playerInfo.playerCamera.Priority = 10;
        }

        public override void Init(PlayerControllerInfo playerInfoManager)
        {
            base.Init(playerInfoManager);
            transposer = playerInfo.playerCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_CameraDistance = initialDistance;
            playerInfo.playerCamera.transform.rotation = Quaternion.Euler(initialAngle, -45f, 0f);
            initialDistanceRatio = initialDistance / cameraDistanceBounds.y;
        }

        public void OnZoom(float scroll)
        {
            float camDist = Mathf.Max(cameraDistanceBounds.x, Mathf.Min(cameraDistanceBounds.y, transposer.m_CameraDistance - scroll * scrollSpeed));

            DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, camDist, 0.1f).SetEase(Ease.InOutSine);

            //playerInfo.playerCamera.transform.rotation = Quaternion.Euler(Mathf.LerpUnclamped(minCameraAngle, initialAngle, transposer.m_CameraDistance / cameraDistanceBounds.y / initialDistanceRatio), -45f, 0f);

            playerInfo.playerCamera.transform.DOLocalRotate(new Vector3(Mathf.LerpUnclamped(minCameraAngle, initialAngle, camDist / cameraDistanceBounds.y / initialDistanceRatio), -45f, 0f), 0.5f).SetEase(Ease.Linear);

            DOTween.To(() => transposer.m_TrackedObjectOffset, x => transposer.m_TrackedObjectOffset = x, Vector3.up * Mathf.LerpUnclamped(minTargetOffset, initialYOffset, camDist / cameraDistanceBounds.y / initialDistanceRatio), 0.1f).SetEase(Ease.InOutSine);
            
            //transposer.m_TrackedObjectOffset = Vector3.up * Mathf.LerpUnclamped(minTargetOffset, initialYOffset, transposer.m_CameraDistance / cameraDistanceBounds.y / initialDistanceRatio);
        }
    }
}
