using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class VirtualCamera : MonoBehaviour
{
    [SerializeField] protected CinemachineVirtualCamera virtualCam;
    [SerializeField] private CameraShake cameraShake;

    [Header("Camera Zoom settings")]
    [SerializeField, ReadOnly] float iniCameraDistance = 15;
    [SerializeField, ReadOnly] CinemachineFramingTransposer cinemachineFraming;
    [SerializeField, ReadOnly] bool isZooming;
    [SerializeField, ReadOnly] Vector3 iniDamping;
    [SerializeField] Vector3 zoomDamping;
    [SerializeField] float resetDampingDelay = 0.5f;

    public CameraShake CameraShake { get => cameraShake; }

    private void Start()
    {
        iniCameraDistance = cinemachineFraming.m_CameraDistance;
        iniDamping = new Vector3(cinemachineFraming.m_XDamping, cinemachineFraming.m_YDamping, cinemachineFraming.m_ZDamping);
    }

    protected virtual void OnEnable()
    {
        CameraManager.instance.AddVirtualCamera(this);
        cinemachineFraming = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
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

    public virtual void SetCameraDamping(Vector3 m_damping)
    {
        cinemachineFraming.m_XDamping = m_damping.x;
        cinemachineFraming.m_YDamping = m_damping.y;
        cinemachineFraming.m_ZDamping = m_damping.z;
    }

    public void Zoom(float targetCameradistance = 10, float zoomDuration = 2)
    {
        if (!cinemachineFraming) return;
        if (isZooming) return;
        StartCoroutine(CameraZoom(targetCameradistance, zoomDuration));
    }

    protected IEnumerator CameraZoom(float p_cameraDistance, float p_zoomDuration)
    {
        isZooming = true;
        SetCameraDamping(zoomDamping);
        cinemachineFraming.m_CameraDistance = p_cameraDistance;
        yield return new WaitForSeconds(p_zoomDuration);
        cinemachineFraming.m_CameraDistance = iniCameraDistance;
        isZooming = false;
        yield return new WaitForSeconds(resetDampingDelay);
        SetCameraDamping(iniDamping);
    }

    [Button]
    public void CamZoomDebug()
    {
        Zoom();
    }
}
