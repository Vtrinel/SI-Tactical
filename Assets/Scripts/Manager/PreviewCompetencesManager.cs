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

    #region Throw

    [Header("Throw")]
    [SerializeField] ShootArrowPreview arrowPreviewShoot;
    public void StartThrowPreview(Vector3 startPos, Vector3 targetPos)
    {
        arrowPreviewShoot.gameObject.SetActive(true);

        discEffectRange = DiscManager.Instance.rangeOfPlayer;
        discEffectZonePreview.gameObject.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * discEffectRange;

        UpdateThrowPreview(startPos, targetPos);
    }

    public void UpdateThrowPreview(Vector3 startPos, Vector3 targetPos)
    {
        arrowPreviewShoot.SetPositions(new List<Vector3> { startPos, targetPos });

        discEffectZonePreview.transform.position = startPos;
    }

    public void EndThrowPreview()
    {
        arrowPreviewShoot.gameObject.SetActive(false);
        discEffectZonePreview.gameObject.SetActive(false);
    }
    #endregion

    #region Recall 

    public void StartRecallPreview(Vector3 recallPosition)
    {
        discEffectRange = DiscManager.Instance.rangeOfPlayer;
        discEffectZonePreview.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * discEffectRange;

        UpdateRecallPreview(recallPosition);
    }

    public void UpdateRecallPreview(Vector3 recallPosition)
    {
        discEffectZonePreview.transform.position = recallPosition;
    }

    public void EndRecallPreview()
    {
        discEffectZonePreview.SetActive(false);
    }
    #endregion
}
