using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineVirtualCamera virtualCamera = default;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    [SerializeField, ReadOnly] private bool isShaking = default;
    [SerializeField, ReadOnly] private float shakeTime = 0f;
    [SerializeField, ReadOnly] ShakeSettings currentShakeSettings = default;

    public bool IsShaking { get => isShaking; private set => isShaking = value; }

    [Header("Debug")]
    [SerializeField] ShakeSettings debugShakeSetting = default;

    // Use this for initialization
    void Start()
    {
        // Get Virtual Camera Noise Profile
        if (virtualCamera != null)
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG
        if (Input.GetKey(KeyCode.S) && debugShakeSetting != null)
        {
            StartShake(debugShakeSetting);
        }

        // If the Cinemachine componet is not set, avoid update
        if (virtualCamera != null && virtualCameraNoise != null && IsShaking)
        {
            // If Camera Shake effect is still playing
            if (shakeTime > 0)
            {
                // Update Shake Timer
                shakeTime -= Time.deltaTime;

                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = currentShakeSettings.GetShakeAmplitude(shakeTime / currentShakeSettings.ShakeDuration);
                virtualCameraNoise.m_FrequencyGain = currentShakeSettings.GetShakeFrequency(shakeTime / currentShakeSettings.ShakeDuration);
            }
            else
            {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_FrequencyGain = 0.0f;
                virtualCameraNoise.m_AmplitudeGain = 0.0f;
                shakeTime = 0f;
                IsShaking = false;
            }
        }
    }

    public void StartShake(ShakeSettings p_shakeSettings)
    {
        currentShakeSettings = p_shakeSettings;
        shakeTime = p_shakeSettings.ShakeDuration; //set duration
        virtualCameraNoise.m_NoiseProfile = p_shakeSettings.NoiseSettingProfile; //set noise profile

        virtualCameraNoise.m_AmplitudeGain = p_shakeSettings.GetShakeAmplitude(0.0f);
        virtualCameraNoise.m_FrequencyGain = p_shakeSettings.GetShakeAmplitude(0.0f);

        IsShaking = true;
    }

    public void StopShake()
    {
        IsShaking = false;
        shakeTime = 0.0f;
    }
}
