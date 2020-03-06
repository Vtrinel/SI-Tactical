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

        for(int i = 0; i < startNumberOfArrowPreview; i++)
        {
            ShootArrowPreview newArrowPreview = Instantiate(arrowPreviewPrefab);
            newArrowPreview.gameObject.SetActive(false);
            arrowPreviews.Add(newArrowPreview);
        }
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        //GameManager.Instance.OnThrowCompetenceSelectionStateChanged += DispayOrHideShootPreview;
    }

    private void OnDisable()
    {
        //GameManager.Instance.OnThrowCompetenceSelectionStateChanged -= DispayOrHideShootPreview;
    }

    void DispayOrHideShootPreview(bool statut)
    {
        arrowPreviewShoot.gameObject.SetActive(statut);
    }

    [Header("Global")]
    [SerializeField] GameObject discEffectZonePreviewPrefab = default;
    GameObject discEffectZonePreview = default;

    float discEffectRange = 0;

    [SerializeField] ShootArrowPreview arrowPreviewPrefab = default;
    [SerializeField] int startNumberOfArrowPreview = 5;
    List<ShootArrowPreview> arrowPreviews = new List<ShootArrowPreview>();
    int currentNumberOfShownArrowPreviews = 0;

    /*#region Preview Camera
    [SerializeField] Transform previewFollowCameraParent = default;
    [SerializeField] VirtualCamera previewFollowCamera = default;

    public void StartPreviewCamera(Vector3 cameraPreviewPos)
    {
        CameraManager.instance.AddVirtualCamera(previewFollowCamera);
        previewFollowCamera.gameObject.SetActive(true);
        UpdatePreviewCamera(cameraPreviewPos);
    }

    public void UpdatePreviewCamera(Vector3 cameraPreviewPos)
    {
        previewFollowCameraParent.position = cameraPreviewPos;
    }

    public void EndPreviewCamera()
    {
        CameraManager.instance.RemoveVirtualCamera(previewFollowCamera);
        previewFollowCamera.gameObject.SetActive(false);
    }
    #endregion*/

    #region Trajectories
    public void UpdateNumberOfShownTrajectories(int newNumber)
    {
        if (currentNumberOfShownArrowPreviews == newNumber)
            return;

        currentNumberOfShownArrowPreviews = newNumber;

        if (newNumber > arrowPreviews.Count)
        {
            for(int i = arrowPreviews.Count; i < newNumber; i++)
            {
                ShootArrowPreview newArrowPreview = Instantiate(arrowPreviewPrefab);
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

            //Debug.Log("Start : " + trajParams.startPosition + "; End : " + trajParams.endPosition);
            arrowPreviews[i].SetPositions(trajParams.trajectoryPositions);
        }
    }
    #endregion

    #region Throw

    [Header("Throw")]
    
    [SerializeField] ShootArrowPreview arrowPreviewShoot;
    public void StartThrowPreview(List<DiscTrajectoryParameters> trajectoryParameters, Vector3 playerPosition)
    {
        //arrowPreviewShoot.gameObject.SetActive(true);

        discEffectRange = DiscManager.Instance.rangeOfPlayer;
        discEffectZonePreview.gameObject.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * discEffectRange;
        discEffectZonePreview.transform.position = playerPosition + Vector3.up * 0.01f;

        //UpdateThrowPreview(startPos, targetPos);

        UpdateNumberOfShownTrajectories(trajectoryParameters.Count);
        UpdateThrowPreview(trajectoryParameters);
    }

    public void UpdateThrowPreview(List<DiscTrajectoryParameters> trajectoryParameters/*Vector3 startPos, Vector3 targetPos*/)
    {
        UpdateTrajectoriesPreview(trajectoryParameters);
        /*arrowPreviewShoot.SetPositions(new List<Vector3> { startPos, targetPos });

        discEffectZonePreview.transform.position = startPos;*/
    }

    public void EndThrowPreview()
    {
        //arrowPreviewShoot.gameObject.SetActive(false);

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
}
