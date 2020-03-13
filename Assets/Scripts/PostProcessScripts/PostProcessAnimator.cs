using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using NaughtyAttributes;

public class PostProcessAnimator : MonoBehaviour
{
    [Header("General settings")]
    [SerializeField, Required] PostProcessVolume volume;
    [SerializeField] float animDuration = 1.0f;
    [SerializeField, ReadOnly] float animCount = 0.0f;
    [SerializeField, ReadOnly] protected bool isAnimating;

    [Header("Bloom settings")]
    [SerializeField, ReadOnly] protected Bloom bloom;

    [SerializeField] protected AnimationCurve bloomAnimation;
    [SerializeField, ReadOnly] protected float minBloomIntensity;
    [SerializeField] protected float maxBloomIntensity;

    [Header("Chromatic Aberration settings")]
    [SerializeField, ReadOnly] protected ChromaticAberration chromaticAberration;

    [SerializeField] protected AnimationCurve chromaticAnimation;
    [SerializeField, ReadOnly] protected float minChromaticIntensity;
    [SerializeField] protected float maxChromaticIntensity;

    [Header("Color Grading settings")]
    [SerializeField, ReadOnly] protected ColorGrading colorGrading;

    [SerializeField] protected AnimationCurve postExposureAnimation;
    [SerializeField, ReadOnly] protected float minPostExposure;
    [SerializeField] protected float maxPostExposure;

    [SerializeField] protected AnimationCurve contrastAnimation;
    [SerializeField, ReadOnly] protected float minContrast;
    [SerializeField] protected float maxContrast;

    [SerializeField] protected AnimationCurve saturationAnimation;
    [SerializeField, ReadOnly] protected float minSaturation;
    [SerializeField] protected float maxSaturation;

    [SerializeField] protected AnimationCurve colorFilterAnimation;
    [SerializeField, ColorUsage(true, true), ReadOnly] protected Color startColorFilter;
    [SerializeField, ColorUsage(true, true)] protected Color endColorFilter;

    [Header("Vignette settings")]
    [SerializeField, ReadOnly] protected Vignette vignette;

    [SerializeField] protected AnimationCurve vignetteAnimation;
    [SerializeField, ReadOnly] protected float minVignetteIntensity;
    [SerializeField] protected float maxVignetteIntensity;

    protected virtual void Start()
    {
        bloom = volume.profile.GetSetting<Bloom>();
        chromaticAberration = volume.profile.GetSetting<ChromaticAberration>();
        colorGrading = volume.profile.GetSetting<ColorGrading>();
        vignette = volume.profile.GetSetting<Vignette>();

        //Set Min values
        minBloomIntensity = bloom.intensity.value;
        minVignetteIntensity = vignette.intensity.value;
        minChromaticIntensity = chromaticAberration.intensity.value;
        minPostExposure = colorGrading.postExposure.value;
        minSaturation = colorGrading.saturation.value;
        minContrast = colorGrading.contrast.value;
        startColorFilter = colorGrading.colorFilter.value;
    }

    protected virtual void Update()
    {
        Animation();
    }

    [Button]
    protected virtual void StartAnim()
    {
        animCount = 0.0f;
        isAnimating = true;
    }

    protected virtual void Animation()
    {
        if (!isAnimating) return;
        animCount += Time.deltaTime;
        float l_alpha = AlphaCount();

        if (bloom != null) bloom.intensity.value = Mathf.Lerp(minBloomIntensity, maxBloomIntensity, bloomAnimation.Evaluate(l_alpha));
        if (chromaticAberration != null) chromaticAberration.intensity.value = Mathf.Lerp(minChromaticIntensity, maxChromaticIntensity, chromaticAnimation.Evaluate(l_alpha));
        if (vignette != null) vignette.intensity.value = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, vignetteAnimation.Evaluate(l_alpha));

        if (colorGrading != null)
        {
            colorGrading.saturation.value = Mathf.Lerp(minSaturation, maxSaturation, saturationAnimation.Evaluate(l_alpha));
            colorGrading.contrast.value = Mathf.Lerp(minContrast, maxContrast, contrastAnimation.Evaluate(l_alpha));
            colorGrading.postExposure.value = Mathf.Lerp(minPostExposure, maxPostExposure, postExposureAnimation.Evaluate(l_alpha));
            colorGrading.colorFilter.value = Color.Lerp(startColorFilter, endColorFilter, colorFilterAnimation.Evaluate(l_alpha));
        }


        if (AlphaCount() >= 1.0f)
        {
            isAnimating = false;
        }
    }

    protected float AlphaCount()
    {
        return animCount / animDuration;
    }
}
