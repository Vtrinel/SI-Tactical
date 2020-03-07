using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscTrajectoryFactory
{
    public static DiscTrajectoryParameters GetTrajectory(CompetenceDisc competence, Vector3 startPosition, Vector3 targetPosition, List<DiscScript> throwedDiscs, List<DiscScript> inRangeDiscs, DiscScript currentDisc)
    {
        DiscTrajectoryParameters discTrajectoryParameters = new DiscTrajectoryParameters();
        List<Vector3> trajectoryPositions = new List<Vector3>();
        Vector3 totalDirection = (targetPosition - startPosition).normalized;

        List<TrajectoryModifier> trajectoryModifiers = competence.GetTrajectoryModifiers;
        LinkedDiscTrajectoryType linkedDiscsTrajectory = LinkedDiscTrajectoryType.None;
        TrajectoryModifierCurved curvedModifier = null;

        foreach (TrajectoryModifier modifier in trajectoryModifiers)
        {
            if (linkedDiscsTrajectory == LinkedDiscTrajectoryType.None)
            {
                TrajectoryModifierLinkedDiscs linkedDiscModifier = modifier as TrajectoryModifierLinkedDiscs;
                if (linkedDiscModifier != null)
                    linkedDiscsTrajectory = linkedDiscModifier.GetLinkedDiscTrajectoryType;
            }

            if (curvedModifier == null)
            {
                TrajectoryModifierCurved foundCurvedModifier = modifier as TrajectoryModifierCurved;
                if (foundCurvedModifier != null)
                    curvedModifier = foundCurvedModifier;
            }
        }

        trajectoryPositions.Add(startPosition);

        #region Discs Link
        switch (linkedDiscsTrajectory)
        {
            case LinkedDiscTrajectoryType.FromOldestToNewest:
                for (int i = throwedDiscs.Count - 1; i >= 0; i--)
                {
                    if(throwedDiscs[i] != currentDisc)
                        trajectoryPositions.Add(throwedDiscs[i].transform.position);
                }

                break;
            case LinkedDiscTrajectoryType.FromNewestToOldest:
                for (int i = 0; i < throwedDiscs.Count; i++)
                {
                    if (throwedDiscs[i] != currentDisc)
                        trajectoryPositions.Add(throwedDiscs[i].transform.position);
                }
                break;
        }
        #endregion

        #region
        if(curvedModifier != null)
        {
            trajectoryPositions.Add(targetPosition);
            List<Vector3> curvedTrajectoryPositions = new List<Vector3>();

            for(int i = 0; i < trajectoryPositions.Count - 1; i++)
            {
                curvedTrajectoryPositions.Add(trajectoryPositions[i]);
                AddCurvedTrajectoryBetweenTwoPoints(ref curvedTrajectoryPositions, trajectoryPositions[i], trajectoryPositions[i + 1], curvedModifier);
            }

            trajectoryPositions = curvedTrajectoryPositions;
        }
        #endregion

        trajectoryPositions.Add(targetPosition);

        discTrajectoryParameters.trajectoryPositions = trajectoryPositions;

        return discTrajectoryParameters;
    }

    #region Tests 
    //Zig Zags funs
    /*Vector3 testPos1 = Vector3.Lerp(startPosition, targetPosition, 0.2f);
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
    discTrajectoryParameters.trajectoryPositions.Add(testPos4);*/
    #endregion
    public static void AddCurvedTrajectoryBetweenTwoPoints(ref List<Vector3> listToModify, Vector3 startPos, Vector3 endPos, TrajectoryModifierCurved modifier)
    {
        float totalDistance = Vector3.Distance(startPos, endPos);
        int numberOfInterpolations = modifier.GetNumberOfInterpolations(totalDistance);
        Vector3 directionVector = (endPos - startPos).normalized;
        Vector3 rightVector = new Vector3(directionVector.z, 0, -directionVector.x);

        for(int i = 1; i < numberOfInterpolations; i++)
        {
            float coeff = (float)i / (float)numberOfInterpolations;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, coeff);

            newPos += rightVector * modifier.GetLateralOffsetAtCoeff(totalDistance, coeff);
            listToModify.Add(newPos);
        }
    }

}


public struct DiscTrajectoryParameters
{
    public List<Vector3> trajectoryPositions;
}