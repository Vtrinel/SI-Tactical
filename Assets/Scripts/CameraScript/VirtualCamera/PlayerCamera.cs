﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;

public class PlayerCamera : VirtualCamera
{
    [Header("Player Cam values")]
    [SerializeField] PlayerController player = default;
    [SerializeField] CameraTarget followTransform = default;

    protected override void OnEnable()
    {
        base.OnEnable();
        player = GameManager.Instance.GetPlayer;
        ResetPlayerCamera();
        followTransform.transform.position = player.transform.position;
        SetCameraTarget(followTransform.transform);
        SetCameraActive();
        
    }

    #region Free Cam
    [Header("Free Cam System")]
    [SerializeField] Transform minimumFreeCameraOffset = default;
    [SerializeField] Transform maximumFreeCameraOffset = default;
    [SerializeField] float minimumFreeCameraMoveSpeed = 2;
    [SerializeField] float maximumFreeCameraMoveSpeed = 5;
    [SerializeField] float keyboardFreeCameraMoveSpeed = 3;

    public void MovePlayerCamera(Vector2 movementInput, bool isKeyboardInput)
    {
        Vector3 rightMovement = transform.right;
        if (movementInput.x != 0)
        {
            rightMovement.y = 0;
            rightMovement.Normalize();
            rightMovement *= Time.deltaTime *
                Mathf.Lerp(minimumFreeCameraMoveSpeed, isKeyboardInput ? keyboardFreeCameraMoveSpeed : maximumFreeCameraMoveSpeed,
                Mathf.Abs(movementInput.x)) * Mathf.Sign(movementInput.x);
        }
        else
        {
            rightMovement = Vector3.zero;
        }

        Vector3 forwardMovement = transform.forward;
        if (movementInput.y != 0)
        {
            forwardMovement.y = 0;
            forwardMovement.Normalize();
            forwardMovement *= Time.deltaTime *
                Mathf.Lerp(minimumFreeCameraMoveSpeed, isKeyboardInput ? keyboardFreeCameraMoveSpeed : maximumFreeCameraMoveSpeed,
                Mathf.Abs(movementInput.y)) * Mathf.Sign(movementInput.y);
        }
        else
        {
            forwardMovement = Vector3.zero;
        }

        Debug.DrawRay(transform.position, (rightMovement).normalized * 10f, Color.red);

        followTransform.transform.position += rightMovement + forwardMovement;
        followTransform.transform.position = new Vector3(
                Mathf.Clamp(followTransform.transform.position.x, minimumFreeCameraOffset.position.x, maximumFreeCameraOffset.position.x),
                followTransform.transform.position.y,
                Mathf.Clamp(followTransform.transform.position.z, minimumFreeCameraOffset.position.z, maximumFreeCameraOffset.position.z));
    }

    public void ResetPlayerCamera()
    {
        StartMovementToward(player.transform);
        //followTransform.parent = player.transform;
        //followTransform.position = player.transform.position;
    }

    #endregion

    #region OtherCameraMovements
    public void AttachFollowTransformTo(Transform newTrToFollow)
    {
        StartMovementToward(newTrToFollow);
    }

    public void AttachFollowTransformTo(Transform newTrToFollow, float forcedDuration, AnimationCurve forcedCurve)
    {
        StartMovementToward(newTrToFollow, forcedDuration, forcedCurve);
    }

    public void InstantPlaceCameraOnTransform(Transform newTrToFollow)
    {
        followTransform.InstantPlace(newTrToFollow);
        SetCameraDamping(Vector3.zero);
        transform.position = followTransform.transform.position - transform.forward * cinemachineFraming.m_CameraDistance;
        SetCameraDamping(iniDamping);
    }
    #endregion

    public void StartMovementToward(Transform tr)
    {
        followTransform.StartMovement(tr);
    }

    public void StartMovementToward(Transform tr, float forcedDuration, AnimationCurve forcedCurve)
    {
        followTransform.StartMovement(tr, forcedDuration, forcedCurve);
    }

    public void SetUpEndMovementEvent(Action endAction)
    {
        followTransform.SetActionToPlayOnEndedMovement(endAction);
    }
}
