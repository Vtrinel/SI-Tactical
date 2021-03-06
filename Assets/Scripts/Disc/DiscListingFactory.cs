﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscListingFactory
{
    public static List<DiscScript> GetSortedRecallableDiscs(CompetenceRecall competence, List<DiscScript> throwedDiscs, List<DiscScript> inRangeDiscs)
    {
        return GetSortedInRangeDiscs(competence.GetNumberOfRecalledDiscs, competence.GetRecallingOrder, 
            competence.GetCanRecallUnthrowedDiscs, throwedDiscs, inRangeDiscs);        
    }

    public static List<DiscScript> GetSortedInRangeDiscs(int numberOfDiscsToRecall, DiscsOrder discsRecallOrder, bool canRecallUnthrowedDiscs, List<DiscScript> throwedDiscs, List<DiscScript> inRangeDiscs)
    {

        List<DiscScript> finalList = new List<DiscScript>();

        int remainingNumberOfDiscsToRecall = numberOfDiscsToRecall;
        bool recallAll = remainingNumberOfDiscsToRecall == 0;

        int currentDiscIndex = (discsRecallOrder == DiscsOrder.FromNewestToOldest ? throwedDiscs.Count - 1 : 0);
        while ((recallAll || remainingNumberOfDiscsToRecall > 0) && throwedDiscs.Count > 0)
        {
            DiscScript currentDisc = throwedDiscs[currentDiscIndex];
            if (!inRangeDiscs.Contains(currentDisc))
            {
                currentDiscIndex += (discsRecallOrder == DiscsOrder.FromNewestToOldest ? -1 : 1);
                if (currentDiscIndex < 0 || currentDiscIndex >= throwedDiscs.Count)
                {
                    break;
                }
                continue;
            }

            finalList.Add(currentDisc);

            currentDiscIndex += (discsRecallOrder == DiscsOrder.FromNewestToOldest ? -1 : 1);
            remainingNumberOfDiscsToRecall--;
            if (currentDiscIndex < 0 || currentDiscIndex >= throwedDiscs.Count)
            {
                break;
            }
        }

        if ((remainingNumberOfDiscsToRecall > 0 || recallAll) && canRecallUnthrowedDiscs)
        {
            foreach (DiscScript disc in inRangeDiscs)
            {
                if (throwedDiscs.Contains(disc))
                    continue;

                finalList.Add(disc);

                remainingNumberOfDiscsToRecall--;
                if (remainingNumberOfDiscsToRecall == 0)
                    break;
            }
        }

        return finalList;
    }


    public static Dictionary<DiscScript, DiscTrajectoryParameters> GetDiscInRangeTrajectory(Vector3 targetPosition, CompetenceRecall recallCompetence)
    {
        List<DiscScript> recallableFromPosition =
            DiscListingFactory.GetSortedRecallableDiscs(recallCompetence,
            DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetAllInRangeDiscsFromPosition(targetPosition));

        Dictionary<DiscScript, DiscTrajectoryParameters> allTrajParams = new Dictionary<DiscScript, DiscTrajectoryParameters>();
        foreach (DiscScript disc in recallableFromPosition)
        {
            DiscTrajectoryParameters newTrajParams =
                DiscTrajectoryFactory.GetTrajectory(recallCompetence, disc.transform.position, targetPosition,
                DiscManager.Instance.GetAllThrowedDiscs, DiscManager.Instance.GetInRangeDiscs, disc);

            allTrajParams.Add(disc, newTrajParams);
        }
        return allTrajParams;
    }
}
