﻿using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCompetencesManager : MonoBehaviour
{
    private static PreviewCompetencesManager _instance;
    public static PreviewCompetencesManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        discEffectZonePreview = Instantiate(discEffectZonePreviewPrefab);
        discEffectZonePreview.SetActive(false);

        InstantiateMovementPreviewElements();
        InstantiateTrajectoryPreviewElements();
    }

    [Header("Global")]
    [SerializeField] GameObject discEffectZonePreviewPrefab = default;
    GameObject discEffectZonePreview = default;

    float discEffectRange = 0;

    #region
    [Header("Movement")]
    [SerializeField] Transform movementPreviewsParent = default;
    [SerializeField] MovementCirclePreview movementCirclePreviewPrefab = default;
    [SerializeField] int startNumberOfMovementCirclePreviews = 5;
    [SerializeField, ReadOnly] List<MovementCirclePreview> movementCirclePreviews = new List<MovementCirclePreview>();

    [SerializeField] MovementLinePreview movementLinePreviewPrefab = default;
    [SerializeField, ReadOnly] MovementLinePreview movementLinePreview = default;

    [SerializeField] MovementGhostPreview movementGhostPreviewPrefab = default;
    [SerializeField, ReadOnly] MovementGhostPreview movementGhostPreview = default;

    public void InstantiateMovementPreviewElements()
    {
        for (int i = 0; i < startNumberOfMovementCirclePreviews; i++)
        {
            MovementCirclePreview newMovementCirclePreview = Instantiate(movementCirclePreviewPrefab, movementPreviewsParent);
            newMovementCirclePreview.HidePreview();
            movementCirclePreviews.Add(newMovementCirclePreview);
        }

        /*movementLinePreview = Instantiate(movementLinePreviewPrefab, movementPreviewsParent);
        movementLinePreview.HidePreview();*/

        movementGhostPreview = Instantiate(movementGhostPreviewPrefab, movementPreviewsParent);
        movementGhostPreview.HidePreview();
    }

    public void StartMovementPreview(List<float> distances, Vector3 startPosition, Vector3 targetPosition, CompetenceRecall currentRecallComp)
    {
        #region Circles
        int newNumber = distances.Count;
        if (newNumber > movementCirclePreviews.Count)
        {
            for (int i = movementCirclePreviews.Count; i < newNumber; i++)
            {
                MovementCirclePreview newMovementCirclePreview = Instantiate(movementCirclePreviewPrefab, movementPreviewsParent);
                newMovementCirclePreview.ShowPreview();
                movementCirclePreviews.Add(newMovementCirclePreview);
            }
        }

        Vector3 circlePos = new Vector3(startPosition.x, 0.01f, startPosition.z);
        for (int i = 0; i < newNumber; i++)
        {
            movementCirclePreviews[i].ShowPreview();
            movementCirclePreviews[i].ChangeRadius(distances[i]);
            movementCirclePreviews[i].transform.position = circlePos;
            Debug.DrawRay(circlePos, Vector3.up * 10, Color.red, 5f);
        }

        for(int i = newNumber; i < movementCirclePreviews.Count; i++)
        {
            movementCirclePreviews[i].HidePreview();
        }
        #endregion

        //movementLinePreview.ShowPreview();
        movementGhostPreview.ShowPreview();
    }

    public void UpdateMovementPreview(Vector3 startPosition, Vector3 targetPosition, CompetenceRecall currentRecallComp)
    {

    }

    public void EndMovementPreview()
    {
        foreach (MovementCirclePreview circlePreview in movementCirclePreviews)
            circlePreview.HidePreview();

        //movementLinePreview.HidePreview();
        movementGhostPreview.HidePreview();
    }
    #endregion

    #region Trajectories - Throw and Recall
    [Header("Trajectories - Throw and Recall")]
    [SerializeField] Transform trajectoryPreviewParent = default;
    [SerializeField] ShootArrowPreview arrowPreviewPrefab = default;
    [SerializeField] int startNumberOfArrowPreview = 5;
    List<ShootArrowPreview> arrowPreviews = new List<ShootArrowPreview>();
    int currentNumberOfShownArrowPreviews = 0;

    public void InstantiateTrajectoryPreviewElements()
    {
        for (int i = 0; i < startNumberOfArrowPreview; i++)
        {
            ShootArrowPreview newArrowPreview = Instantiate(arrowPreviewPrefab, trajectoryPreviewParent);
            newArrowPreview.gameObject.SetActive(false);
            arrowPreviews.Add(newArrowPreview);
        }
    }

    public void UpdateNumberOfShownTrajectories(int newNumber)
    {
        if (currentNumberOfShownArrowPreviews == newNumber)
            return;

        currentNumberOfShownArrowPreviews = newNumber;

        if (newNumber > arrowPreviews.Count)
        {
            for(int i = arrowPreviews.Count; i < newNumber; i++)
            {
                ShootArrowPreview newArrowPreview = Instantiate(arrowPreviewPrefab, trajectoryPreviewParent);
                newArrowPreview.gameObject.SetActive(false);
                arrowPreviews.Add(newArrowPreview);
            }
        }

        for(int i = 0; i < arrowPreviews.Count; i++)
        {
            arrowPreviews[i].gameObject.SetActive(i < newNumber);
        }
    }

    public void UpdateTrajectoriesPreview(List<DiscTrajectoryParameters> discTrajectories)
    {
        DiscTrajectoryParameters trajParams = default;
        for (int i = 0; i < discTrajectories.Count; i++)
        {
            if(i > currentNumberOfShownArrowPreviews)
            {
                Debug.LogWarning("WARNING : Updating more arrows than shown");
            }

            trajParams = discTrajectories[i];

            arrowPreviews[i].SetPositions(trajParams.trajectoryPositions);
        }
    }

    #region Throw
    public void StartThrowPreview(List<DiscTrajectoryParameters> trajectoryParameters, Vector3 playerPosition)
    {
        discEffectRange = DiscManager.Instance.rangeOfPlayer;
        discEffectZonePreview.gameObject.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * discEffectRange;
        discEffectZonePreview.transform.position = playerPosition + Vector3.up * 0.01f;

        UpdateNumberOfShownTrajectories(trajectoryParameters.Count);
        UpdateThrowPreview(trajectoryParameters);
    }

    public void UpdateThrowPreview(List<DiscTrajectoryParameters> trajectoryParameters/*Vector3 startPos, Vector3 targetPos*/)
    {
        UpdateTrajectoriesPreview(trajectoryParameters);
    }

    public void EndThrowPreview()
    {
        UpdateNumberOfShownTrajectories(0);
        discEffectZonePreview.gameObject.SetActive(false);
    }
    #endregion

    #region Recall 

    public void StartRecallPreview(List<DiscTrajectoryParameters> trajectoryParameters, Vector3 recallPosition)
    {
        discEffectRange = DiscManager.Instance.rangeOfPlayer;
        discEffectZonePreview.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * discEffectRange;
        discEffectZonePreview.transform.position = recallPosition + Vector3.up * 0.01f;

        UpdateNumberOfShownTrajectories(trajectoryParameters.Count);
        UpdateRecallPreview(trajectoryParameters, recallPosition);
    }

    public void UpdateRecallPreview(List<DiscTrajectoryParameters> trajectoryParameters, Vector3 recallPosition)
    {
        discEffectZonePreview.transform.position = recallPosition + Vector3.up * 0.01f;

        if (trajectoryParameters.Count != currentNumberOfShownArrowPreviews)
            UpdateNumberOfShownTrajectories(trajectoryParameters.Count);

        UpdateTrajectoriesPreview(trajectoryParameters);
    }

    public void EndRecallPreview()
    {
        discEffectZonePreview.SetActive(false);
        UpdateNumberOfShownTrajectories(0);
    }
    #endregion
    #endregion
}
