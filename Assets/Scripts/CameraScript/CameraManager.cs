using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CameraManager : MonoBehaviour
{
    [SerializeField] List<VirtualCamera> virtualCameras;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField, ReadOnly] VirtualCamera currentCamera;
    [SerializeField, ReadOnly] int activeCamPriority = 10;
    [SerializeField, ReadOnly] int inactiveCamPriority = 0;

    public static CameraManager instance;
    public PlayerCamera GetPlayerCamera => playerCamera;

    [Header("Debug")]
    [SerializeField] int debugCamID;

    public List<VirtualCamera> VirtualCameras { get => virtualCameras; private set => virtualCameras = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void SetCamera(int camID)
    {
        for (int i = 0; i < virtualCameras.Count; i++)
        {
            VirtualCameras[i].SetPriority(i == camID ? activeCamPriority : inactiveCamPriority);
        }

        currentCamera = VirtualCameras[camID];
    }

    public void SetCamera(VirtualCamera vCam)
    {
        //if (isPlayerCam) playerCamera = (PlayerCamera)p_virtualCam;
        for (int i = 0; i < virtualCameras.Count; i++)
        {
            VirtualCameras[i].SetPriority(VirtualCameras[i] == vCam ? activeCamPriority : inactiveCamPriority);
            Debug.Log("Set Cam");
        }

        currentCamera = vCam;
    }

    public void AddVirtualCamera(VirtualCamera p_virtualCam, bool isPlayerCam = false)
    {
        VirtualCameras.Add(p_virtualCam);
        if (isPlayerCam) playerCamera = (PlayerCamera)p_virtualCam;
    }

    public void RemoveVirtualCamera(VirtualCamera p_virtualCam)
    {
        if (!VirtualCameras.Contains(p_virtualCam)) return;

        VirtualCameras.Remove(p_virtualCam);
    }

    [Button]
    void DebugSetCam()
    {
        SetCamera(debugCamID);
    }
}
