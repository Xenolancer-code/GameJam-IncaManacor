using System;
using Unity.Cinemachine;
using UnityEngine;

public class ChangeCameraAngle : MonoBehaviour
{
    private int camActive=1;
    private int camInactive=0;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera playerCameraToBoss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerCamera.Priority=camInactive;
            playerCameraToBoss.Priority=camActive;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerCamera.Priority=camActive;
        playerCameraToBoss.Priority=camInactive;
    }
}
