using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscTrajectoryFactory
{
    public static DiscTrajectoryParameters GetThrowTrajectory(CompetenceThrow competence, Vector3 startPosition, Vector3 targetPosition, List<DiscScript> throwedDiscs, List<DiscScript> inRangeDiscs)
    {
        DiscTrajectoryParameters discTrajectoryParameters = new DiscTrajectoryParameters();
        discTrajectoryParameters.trajectoryPositions = new List<Vector3>();

        discTrajectoryParameters.trajectoryPositions.Add(startPosition);

        Vector3 totalDirection = (targetPosition - startPosition).normalized;

        Vector3 testPos1 = Vector3.Lerp(startPosition, targetPosition, 0.2f);
        testPos1 += new Vector3(totalDirection.z, 0, -totalDirection.x).normalized * 1f;
        Vector3 testPos2 = Vector3.Lerp(startPosition, targetPosition, 0.4f);
        testPos2 += new Vector3(-totalDirection.z, 0, totalDirection.x).normalized * 1f;

        Vector3 testPos3 = Vector3.Lerp(startPosition, targetPosition, 0.6f);
        testPos3 += new Vector3(totalDirection.z, 0, -totalDirection.x).normalized * 1f;
        Vector3 testPos4 = Vector3.Lerp(startPosition, targetPosition, 0.8f);
        testPos4 += new Vector3(-totalDirection.z, 0, totalDirection.x).normalized * 1f;

        discTrajectoryParameters.trajectoryPositions.Add(testPos1);
        discTrajectoryParameters.trajectoryPositions.Add(testPos2);
        discTrajectoryParameters.trajectoryPositions.Add(testPos3);
        discTrajectoryParameters.trajectoryPositions.Add(testPos4);

        discTrajectoryParameters.trajectoryPositions.Add(targetPosition);

        return discTrajectoryParameters;
    }

    public static DiscTrajectoryParameters GetRecallTrajectory(CompetenceRecall competence, Vector3 startPosition, Vector3 targetPosition, List<DiscScript> throwedDiscs, List<DiscScript> inRangeDiscs)
    {
        DiscTrajectoryParameters discTrajectoryParameters = new DiscTrajectoryParameters();
        discTrajectoryParameters.trajectoryPositions = new List<Vector3>();

        discTrajectoryParameters.trajectoryPositions.Add(startPosition);

        Vector3 totalDirection = (targetPosition - startPosition).normalized;

        Vector3 testPos1 = Vector3.Lerp(startPosition, targetPosition, 0.2f);
        testPos1 += new Vector3(totalDirection.z, 0, -totalDirection.x).normalized * 1f;
        Vector3 testPos2 = Vector3.Lerp(startPosition, targetPosition, 0.4f);
        testPos2 += new Vector3(-totalDirection.z, 0, totalDirection.x).normalized * 1f;

        Vector3 testPos3 = Vector3.Lerp(startPosition, targetPosition, 0.6f);
        testPos3 += new Vector3(totalDirection.z, 0, -totalDirection.x).normalized * 1f;
        Vector3 testPos4 = Vector3.Lerp(startPosition, targetPosition, 0.8f);
        testPos4 += new Vector3(-totalDirection.z, 0, totalDirection.x).normalized * 1f;

        discTrajectoryParameters.trajectoryPositions.Add(testPos1);
        discTrajectoryParameters.trajectoryPositions.Add(testPos2);
        discTrajectoryParameters.trajectoryPositions.Add(testPos3);
        discTrajectoryParameters.trajectoryPositions.Add(testPos4);

        discTrajectoryParameters.trajectoryPositions.Add(targetPosition);

        return discTrajectoryParameters;
    }
}

public struct DiscTrajectoryParameters
{
    public List<Vector3> trajectoryPositions;
}