using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : VirtualCamera
{
    [Header("Player Cam values")]
    [SerializeField] PlayerController player;

    protected override void OnEnable()
    {
        base.OnEnable();
        player = GameManager.Instance.GetPlayer;
        SetCameraTarget(player.transform);
        SetCameraActive();
    }
}
