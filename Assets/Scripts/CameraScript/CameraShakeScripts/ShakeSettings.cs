using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ShakeSetting",menuName = "ScriptableObject/ShakeSetting")]
public class ShakeSettings : ScriptableObject
{
    [Header("Shake settings")]
    [SerializeField] NoiseSettings noiseSettingProfile;

    [Min(0.0f), SerializeField] float shakeDuration = 0.3f; // Time the Camera Shake effect will last
    [Min(0.0f), SerializeField] float shakeAmplitude = 1.2f; // Cinemachine Noise Profile Parameter
    [Min(0.0f), SerializeField] float shakeFrequency = 2.0f; // Cinemachine Noise Profile Parameter

    [SerializeField] AnimationCurve amplitudeOverTime;
    [SerializeField] AnimationCurve frequencyOverTime;

    public NoiseSettings NoiseSettingProfile { get => noiseSettingProfile; private set => noiseSettingProfile = value; }
    public float ShakeDuration { get => shakeDuration; private set => shakeDuration = value; }
    public float ShakeAmplitude { get => shakeAmplitude; private set => shakeAmplitude = value; }
    public float ShakeFrequency { get => shakeFrequency; private set => shakeFrequency = value; }
    public AnimationCurve AmplitudeOverTime { get => amplitudeOverTime; private set => amplitudeOverTime = value; }
    public AnimationCurve FrequencyOverTime { get => frequencyOverTime; private set => frequencyOverTime = value; }

    public float GetShakeAmplitude(float amplitudeTime)
    {
        return ShakeAmplitude * amplitudeOverTime.Evaluate(amplitudeTime);
    }

    public float GetShakeFrequency(float frequencyTime)
    {
        return ShakeFrequency * frequencyOverTime.Evaluate(frequencyTime);
    }
}
