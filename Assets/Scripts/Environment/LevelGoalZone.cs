using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGoalZone : MonoBehaviour
{
    public void SetUp()
    {
        if (debugZone != null)
        {
            debugZone.transform.localScale = Vector3.one * zoneRadius;
            debugZone.color = normalColor;
        }
        if (progressBar) progressBar.fillAmount = 0;
    }

    [SerializeField] ParticleSystem particlesToPlayOnWin = default;

    public void PlayWinParticles()
    {
        if (particlesToPlayOnWin != null)
            particlesToPlayOnWin.gameObject.SetActive(true);
    }
    [SerializeField] Transform transformToLookAt = default;
    public Transform GetTransformToLookAt { get { if (transformToLookAt != null) return transformToLookAt; else return transform; } }

    [Header("Main Parameters")]
    [SerializeField] float zoneRadius = 8f;

    [Header("Debug")]
    [SerializeField] SpriteRenderer debugZone = default;
    [SerializeField] Color normalColor = Color.grey;
    [SerializeField] Color playerIsInRangeColor = Color.yellow;
    [SerializeField] Image progressBar = default;

    [Header("Tooltips")]
    [SerializeField] TooltipCollider tooltipCollider = default;

    bool playerEnteredOnce = false;
    bool playerIsInRange = false;
    public void CheckIfPlayerIsInRange(Vector3 playerPos)
    {
        ChangePlayerIsInRange(GetDistanceWithPlayer(playerPos) <= zoneRadius);
    }
    public float GetDistanceWithPlayer(Vector3 playerPos)
    {
        Vector3 selfPos = transform.position;
        playerPos.y = transform.position.y;

        return Vector3.Distance(playerPos, selfPos);
    }

    public void ChangePlayerIsInRange(bool inRange)
    {
        if (inRange == playerIsInRange)
            return;

        playerIsInRange = inRange;

        if (debugZone != null)
            debugZone.color = playerIsInRange ? playerIsInRangeColor : normalColor;

        if (!playerEnteredOnce)
        {
            playerEnteredOnce = true;
            UIManager.Instance.OnGoalZoneReached();
            UpdateDescription(LevelProgressionManager.Instance.GetRemainingNumberOfTurn);
            SoundManager.Instance.PlayMusic(Music.InBoss);
        }
    }

    public int GetProgressionAmount()
    {
        return playerIsInRange ? 1 : 0;
    }

    bool fireSoundPlayed = false;
    public void UpdateProgressionBar(int currentProgress, int deltaProgress, int targetProgress)
    {
        UpdateDescription(LevelProgressionManager.Instance.GetRemainingNumberOfTurn);
        if (progressBar != null)
        {
            progressBar.fillAmount = (float)currentProgress / targetProgress;
        }

        if(currentProgress > 0 && !fireSoundPlayed)
        {
            fireSoundPlayed = true;
            SoundManager.Instance.PlayMusic(Music.fireLightUp);
        }
        SoundManager.Instance.PlaySound(Sound.statueCrack, gameObject.transform.position);
    }

    private void Start()
    {
        SetStartDescription();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerPositionChanged += CheckIfPlayerIsInRange;
        if (tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip += StartTooltip;
            tooltipCollider.OnEndTooltip += EndTootlip;
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerPositionChanged -= CheckIfPlayerIsInRange;
        if (tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip -= StartTooltip;
            tooltipCollider.OnEndTooltip -= EndTootlip;
        }
    }

    public void StartTooltip()
    {

    }

    public void EndTootlip()
    {

    }

    public void SetStartDescription()
    {
        if (tooltipCollider != null)
        {
            tooltipCollider.SetDescription("Enter the area to start the corruption of the God's Statue");
        }
        else
            Debug.Log("Enter the area to start the corruption of the God's Statue");
    }

    public void UpdateDescription(int turnValue)
    {
        if (tooltipCollider != null)
        {
            tooltipCollider.SetDescription("Stay near the statue for " + turnValue + " more turns to complete corruption");
        }
        else
            Debug.Log("Stay near the statue for " + turnValue + " more turns to complete corruption");
    }
}
