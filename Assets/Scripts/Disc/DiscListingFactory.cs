using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscListingFactory
{
    public static List<DiscScript> GetSortedRecallableDiscs(CompetenceRecall competence, List<DiscScript> throwedDiscs, List<DiscScript> inRangeDiscs)
    {
        List<DiscScript> finalList = new List<DiscScript>();

        int remainingNumberOfDiscsToRecall = competence.GetNumberOfRecalledDiscs;
        bool recallAll = remainingNumberOfDiscsToRecall == 0;
        DiscsOrder discsRecallOrder = competence.GetRecallingOrder;
        bool canRecallUnthrowedDiscs = competence.GetCanRecallUnthrowedDiscs;

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
}
