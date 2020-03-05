using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCompetencesManager : MonoBehaviour
{
    [SerializeField] GameObject arrowPreviewShoot;


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

    }

    private void OnEnable()
    {
        GameManager.Instance.OnThrowCompetenceSelectionStateChanged += DispayOrHideShootPreview;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnThrowCompetenceSelectionStateChanged -= DispayOrHideShootPreview;
    }

    void DispayOrHideShootPreview(bool statut)
    {
        arrowPreviewShoot.SetActive(statut);
    }

}
