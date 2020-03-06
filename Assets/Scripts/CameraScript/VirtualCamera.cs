using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCamera : MonoBehaviour
{
    [SerializeField] protected CinemachineVirtualCamera virtualCam;
    [SerializeField] private CameraShake cameraShake;

    public CameraShake CameraShake { get => cameraShake; }

    protected virtual void OnEnable()
    {
        CameraManager.instance.AddVirtualCamera(this);
    }
    protected virtual void OnDisable()
    {
        CameraManager.instance.RemoveVirtualCamera(this);
    }

    public virtual void SetPriority(int p_priority)
    {
        virtualCam.Priority = p_priority;
    }

    public virtual void SetCameraTarget(Transform target)
    {
        virtualCam.Follow = target;
        //virtualCam.LookAt = target;
    }

    public virtual void SetCameraActive()
    {
        CameraManager.instance.SetCamera(this);
    }
}
