using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : VirtualCamera
{
    public void Move()
    {
        
    }

    public override void SetCameraActive()
    {
        HardResetToPlayer(); //Reset Position Before activating this camera
        base.SetCameraActive();
    }

    public void HardResetToPlayer()
    {
        if (!CameraManager.instance.GetPlayerCamera) return; //PlayerCamera not found

        transform.position = CameraManager.instance.GetPlayerCamera.transform.position;
    }


}
