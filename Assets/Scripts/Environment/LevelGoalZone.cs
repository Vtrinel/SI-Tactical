using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGoalZone : MonoBehaviour
{
    public void SetUp()
    {
        debugZone.transform.localScale = Vector3.one * zoneRadius;
        debugZone.color = normalColor;
        progressBar.fillAmount = 0;
    }

    [Header("Main Parameters")]
    [SerializeField] float zoneRadius = 8f;

    [Header("Debug")]
    [SerializeField] SpriteRenderer debugZone = default;
    [SerializeField] Color normalColor = Color.grey;
    [SerializeField] Color playerIsInRangeColor = Color.yellow;
    [SerializeField] Image progressBar = default;

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

        debugZone.color = playerIsInRange ? playerIsInRangeColor : normalColor;
    }

    public int GetProgressionAmount()
    {
        return playerIsInRange ? 1 : 0;
    }

    public void UpdateProgressionBar(int currentProgress, int deltaProgress, int targetProgress)
    {
        if(progressBar != null)
        {
            progressBar.fillAmount = (float)currentProgress / targetProgress;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerPositionChanged += CheckIfPlayerIsInRange;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerPositionChanged -= CheckIfPlayerIsInRange;
    }
}
