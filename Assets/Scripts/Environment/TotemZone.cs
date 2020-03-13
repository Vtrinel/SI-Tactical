using UnityEngine;
using UnityEngine.UI;

public class TotemZone : MonoBehaviour
{
    public void SetUp()
    {
        debugZone.transform.localScale = Vector3.one * zoneRadius;
        debugZone.color = normalColor;
    }

    [Header("Main Parameters")]
    [SerializeField] float zoneRadius = 2.5f;

    [Header("Debug")]
    [SerializeField] SpriteRenderer debugZone = default;
    [SerializeField] Color normalColor = Color.grey;
    [SerializeField] Color playerIsInRangeColor = Color.blue;
    PlayerController playerController = default;

    private void Start()
    {
        playerController = GameManager.Instance.GetPlayer;
    }

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

        print("test");

        playerIsInRange = inRange;

        debugZone.color = playerIsInRange ? playerIsInRangeColor : normalColor;

        UIManager.Instance.ShowSkillButton(playerIsInRange);

        PlayerExperienceManager.Instance.CanOpenCompetenceMenu(playerIsInRange);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerPositionChanged += CheckIfPlayerIsInRange;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPlayerPositionChanged -= CheckIfPlayerIsInRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}
