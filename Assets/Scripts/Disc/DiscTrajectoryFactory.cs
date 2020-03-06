using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscTrajectoryFactory
{
    public static DiscTrajectoryParameters GetThrowTrajectory(CompetenceThrow competence, Vector3 startPosition, Vector3 targetPosition)
    {
        DiscTrajectoryParameters discTrajectoryParameters = new DiscTrajectoryParameters();

        discTrajectoryParameters.startPosition = startPosition;
        discTrajectoryParameters.trajectoryPositions = new List<Vector3>();
        discTrajectoryParameters.endPosition = targetPosition;

        return discTrajectoryParameters;
    }

    public static DiscTrajectoryParameters GetRecallTrajectory(CompetenceRecall competence, Vector3 startPosition, Vector3 targetPosition)
    {
        DiscTrajectoryParameters discTrajectoryParameters = new DiscTrajectoryParameters();

        discTrajectoryParameters.startPosition = startPosition;
        discTrajectoryParameters.trajectoryPositions = new List<Vector3>();
        discTrajectoryParameters.endPosition = targetPosition;

        return discTrajectoryParameters;
    }
}

public struct DiscTrajectoryParameters
{
    public Vector3 startPosition;
    public List<Vector3> trajectoryPositions;
    public Vector3 endPosition;
}
