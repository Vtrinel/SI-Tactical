using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curved Trajectory", menuName = "Competence/Trajectory Modifiers/Curved")]
public class TrajectoryModifierCurved : TrajectoryModifier
{
    [Header("Curve")]
    [SerializeField] AnimationCurve trajectoryCurve = AnimationCurve.Constant(0, 1, 0);
    [SerializeField] int minimumNumberOfInterpolationsPoints = 5;
    [SerializeField] float distanceBetweenEachInterpolation = 0.33f;

    [Header("Minimum trajectory distance parameters")]
    [SerializeField] float minTrajectoryDistance = 1;
    [SerializeField] float minTrajectoryDistanceLateralOffset = 1;

    [Header("Minimum trajectory distance parameters")]
    [SerializeField] float maxTrajectoryDistance = 10;
    [SerializeField] float maxTrajectoryDistanceLateralOffset = 5;

    public float GetLateralCoeffOffsetWithProgressionCoeff(float coeff) { return trajectoryCurve.Evaluate(coeff); }

    public float GetMaxLateralOffset(float totalDistance)
    {
        float coeff = Mathf.Clamp(1 - (maxTrajectoryDistance - totalDistance)/(maxTrajectoryDistance - minTrajectoryDistance), 0, 1);
        return Mathf.Lerp(minTrajectoryDistanceLateralOffset, maxTrajectoryDistanceLateralOffset, coeff);
    }

    public float GetLateralOffsetAtCoeff(float totalDistance, float coeff)
    {
        return GetMaxLateralOffset(totalDistance) * GetLateralCoeffOffsetWithProgressionCoeff(coeff);
    }

    public int GetNumberOfInterpolations(float totalDistance)
    {
        return Mathf.Clamp(Mathf.RoundToInt(totalDistance/distanceBetweenEachInterpolation) , minimumNumberOfInterpolationsPoints, int.MaxValue);
    }
}
