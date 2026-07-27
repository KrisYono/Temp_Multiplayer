using Unity.Netcode;
using UnityEngine;

public class PlayerCameraControl : NetworkBehaviour
{
    private Cinemachine.CinemachineFreeLook freeLookCamera;

    private void Start()
    {
        if (!IsOwner) return;

        // 找到 FreeLook 摄像机并激活
        freeLookCamera = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        if (freeLookCamera != null)
        {
            freeLookCamera.Follow = transform;
            freeLookCamera.LookAt = transform;
            freeLookCamera.enabled = true;
        }
    }
}