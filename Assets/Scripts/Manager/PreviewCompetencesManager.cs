using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    float discThrowRange = 0;
    float discRecallRange = 0;

    #region Movement
    [Header("Movement")]
    [SerializeField] Transform movementPreviewsParent = default;
    [SerializeField] bool showMovementCircles = false;
    [SerializeField] MovementCirclePreview movementCirclePreviewPrefab = default;
    [SerializeField] int startNumberOfMovementCirclePreviews = 5;
    [SerializeField, ReadOnly] List<MovementCirclePreview> movementCirclePreviews = new List<MovementCirclePreview>();

    [SerializeField] MovementLinePreview movementLinePreviewPrefab = default;
    [SerializeField, ReadOnly] MovementLinePreview movementLinePreview = default;

    [SerializeField] MovementGhostPreview movementGhostPreviewPrefab = default;
    [SerializeField, ReadOnly] MovementGhostPreview movementGhostPreview = default;
    [SerializeField] Material baseGhostMaterial = default;
    [SerializeField] Material cantMoveThereMaterial = default;

    public void InstantiateMovementPreviewElements()
    {
        for (int i = 0; i < startNumberOfMovementCirclePreviews; i++)
        {
            MovementCirclePreview newMovementCirclePreview = Instantiate(movementCirclePreviewPrefab, movementPreviewsParent);
            newMovementCirclePreview.HidePreview();
            movementCirclePreviews.Add(newMovementCirclePreview);
        }

        movementLinePreview = Instantiate(movementLinePreviewPrefab, movementPreviewsParent);
        movementLinePreview.HidePreview();

        movementGhostPreview = Instantiate(movementGhostPreviewPrefab, movementPreviewsParent);
        movementGhostPreview.HidePreview();
        baseGhostMaterial = movementGhostPreview.GetRenderer.material;

    }

    bool justStartedMovementPreview = default;
    List<float> currentMovementDistances = new List<float>();
    public void StartMovementPreview(List<float> distances, List<Vector3> trajectory, CompetenceRecall currentRecallComp, int completelyUsedActionPoints, bool reachMax)
    {
        hitByDiscTrajectories = new Dictionary<DiscScript, Dictionary<GameObject, EnemyBase>>();
        
        movementGhostPreview.GetRenderer.material = baseGhostMaterial;

        Vector3 startPosition = trajectory[0];
        Vector3 targetPosition = trajectory[trajectory.Count - 1];

        #region Circles
        if (showMovementCircles)
        {
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
            }

            for (int i = newNumber; i < movementCirclePreviews.Count; i++)
            {
                movementCirclePreviews[i].HidePreview();
            }
        }
        #endregion

        currentMovementDistances = distances;

        movementLinePreview.ShowPreview();
        movementLinePreview.UpdateLine(trajectory, currentMovementDistances, completelyUsedActionPoints, reachMax);

        movementGhostPreview.ShowPreview();
        movementGhostPreview.transform.position = trajectory[trajectory.Count - 1];

        Dictionary<DiscScript, DiscTrajectoryParameters> discsInNewPositionRangeParameters = DiscListingFactory.GetDiscInRangeTrajectory(targetPosition, currentRecallComp);
        StartRecallPreview(discsInNewPositionRangeParameters, targetPosition);

        justStartedMovementPreview = true;
        UpdateMovementPreview(trajectory, currentRecallComp, completelyUsedActionPoints, reachMax);

        foreach(EnemyBase enemy in EnemiesManager.Instance.GetAllInGameEnemiesOrdered)
        {
            enemy.DisplayAndActualisePreviewAttack(movementGhostPreview.transform);
        }
    }

    public void UpdateMovementPreview(List<Vector3> trajectory, CompetenceRecall currentRecallComp, int completelyUsedActionPoints, bool reachMax)
    {
        Vector3 startPosition = trajectory[0];
        Vector3 targetPosition = trajectory[trajectory.Count - 1];

        movementLinePreview.UpdateLine(trajectory, currentMovementDistances, completelyUsedActionPoints, reachMax);
        movementGhostPreview.transform.position = targetPosition;

        if (!justStartedMovementPreview)
        {
            Dictionary<DiscScript, DiscTrajectoryParameters> discsInNewPositionRangeParameters = DiscListingFactory.GetDiscInRangeTrajectory(targetPosition, currentRecallComp);
            UpdateRecallPreview(discsInNewPositionRangeParameters, targetPosition);
        }
        else
            justStartedMovementPreview = false;
    }

    public void EndMovementPreview()
    {
        ClearHitByDiscTrajectories();

        if (showMovementCircles)
        {
            foreach (MovementCirclePreview circlePreview in movementCirclePreviews)
                circlePreview.HidePreview();
        }

        movementLinePreview.HidePreview();
        movementGhostPreview.HidePreview();

        EndRecallPreview();

        foreach (EnemyBase enemy in EnemiesManager.Instance.GetAllInGameEnemiesOrdered)
        {
            enemy.HidePreview(false);
        }
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

    Dictionary<DiscScript, Dictionary<GameObject, EnemyBase>> hitByDiscTrajectories = new Dictionary<DiscScript, Dictionary<GameObject, EnemyBase>>();
    public void ClearHitByDiscTrajectories()
    {
        foreach(Dictionary<GameObject, EnemyBase> dico in hitByDiscTrajectories.Values)
        {
            foreach(EnemyBase enemy in dico.Values)
            {
                enemy.HideWillBeHitIndicator();
            }
        }

        hitByDiscTrajectories = new Dictionary<DiscScript, Dictionary<GameObject, EnemyBase>>();
    }
    public void UpdateTrajectoriesPreview(Dictionary<DiscScript, DiscTrajectoryParameters> discTrajectories)
    {
        List<DiscScript> previousDiscsUntreated = new List<DiscScript>();
        foreach (DiscScript discToTreat in hitByDiscTrajectories.Keys)
            previousDiscsUntreated.Add(discToTreat);


        DiscTrajectoryParameters trajParams = default;
        List<Vector3> treatedPositions = new List<Vector3>();
        RaycastHit[] hitResults = new RaycastHit[0];

        Vector3 checkStartPos = Vector3.zero;
        Vector3 checkEndPos = Vector3.zero;
        Vector3 checkMovement = Vector3.zero;
        Vector3 discLocalCenter = Vector3.zero;
        LayerMask checkMask = default;
        float checkRadius = default;
        bool blockedByEnemies = false;
        Transform touchedEnemy = default;

        int counter = 0;
        foreach(DiscScript disc in discTrajectories.Keys)
        {
            if (previousDiscsUntreated.Contains(disc))
                previousDiscsUntreated.Remove(disc);

            if (!hitByDiscTrajectories.ContainsKey(disc))
                hitByDiscTrajectories.Add(disc, new Dictionary<GameObject, EnemyBase>());

            List<GameObject> previousEnemiesUntreated = new List<GameObject>();
            foreach (GameObject enemyToTreat in hitByDiscTrajectories[disc].Keys)
                previousEnemiesUntreated.Add(enemyToTreat);

            if (counter > currentNumberOfShownArrowPreviews)
            {
                Debug.LogWarning("WARNING : Updating more arrows than shown");
            }

            trajParams = discTrajectories[disc];
            treatedPositions = new List<Vector3>();
            hitResults = new RaycastHit[0];
            discLocalCenter = trajParams.disc.GetColliderLocalCenter;
            checkMask = trajParams.disc.GetTrajectoryCheckLayerMask;
            checkRadius = trajParams.disc.GetColliderRadius;
            blockedByEnemies = trajParams.disc.GetBlockedByEnemies;
            touchedEnemy = null;

            treatedPositions.Add(trajParams.trajectoryPositions[0]);
            bool goesThrough = true;
            #region Treat Positions
            for (int j = 1; j < trajParams.trajectoryPositions.Count; j++)
            {
                checkStartPos = trajParams.trajectoryPositions[j - 1] + discLocalCenter;
                checkEndPos = trajParams.trajectoryPositions[j] + discLocalCenter;
                checkMovement = checkEndPos - checkStartPos;

                #region Test
                hitResults = Physics.SphereCastAll(checkStartPos, checkRadius, checkMovement.normalized, checkMovement.magnitude, checkMask);
                foreach (RaycastHit hitResult in hitResults.OrderBy(h=>h.distance))
                {
                    GameObject hitObject = hitResult.collider.gameObject;
                    switch (hitObject.layer)
                    {
                        // Enemy
                        case (10):
                            if (!hitByDiscTrajectories[disc].ContainsKey(hitObject))
                            {
                                EnemyBase hitEnemy = hitObject.GetComponent<EnemyBase>();
                                if(hitEnemy != null)
                                {
                                    hitEnemy.ShowWillBeHitIndicator();
                                    hitByDiscTrajectories[disc].Add(hitObject, hitEnemy);
                                }
                            }
                            else
                            {
                                previousEnemiesUntreated.Remove(hitObject);
                            }

                            if (blockedByEnemies)
                            {
                                goesThrough = false;
                            }
                            else
                            {
                                goesThrough = true;
                                touchedEnemy = hitResult.collider.transform;
                            }
                            break;

                        // Shield
                        case (12):
                            goesThrough = false;
                            if (hitResult.collider.transform.parent != null)
                            {
                                if (hitResult.collider.transform.parent.parent != null)
                                {
                                    if (hitResult.collider.transform.parent.parent == touchedEnemy)
                                    {
                                        goesThrough = true;
                                    }
                                }
                            }
                            break;

                        // Obstacle
                        case (14):
                            goesThrough = false;
                            break;
                    }
                    if (!goesThrough)
                    {
                        treatedPositions.Add(hitResult.point - discLocalCenter);
                        break;
                    }
                }

                if (!goesThrough)
                    break;
                #endregion
                
                #endregion
                treatedPositions.Add(trajParams.trajectoryPositions[j]);
            }
            #endregion

            foreach (GameObject untreatedEnemy in previousEnemiesUntreated)
            {
                hitByDiscTrajectories[disc][untreatedEnemy].HideWillBeHitIndicator();
                hitByDiscTrajectories[disc].Remove(untreatedEnemy);
            }

            arrowPreviews[counter].SetPositions(treatedPositions);
            counter++;
        }

        foreach (DiscScript untreatedDisc in previousDiscsUntreated)
        {
            foreach (EnemyBase enemy in hitByDiscTrajectories[untreatedDisc].Values)
            {
                enemy.HideWillBeHitIndicator();
            }
        }
    }

    #region Throw
    public void StartThrowPreview(Dictionary<DiscScript, DiscTrajectoryParameters> trajectoryParameters, Vector3 playerPosition)
    {
        hitByDiscTrajectories = new Dictionary<DiscScript, Dictionary<GameObject, EnemyBase>>();

        discThrowRange = DiscManager.Instance.throwRange;
        discEffectZonePreview.gameObject.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * discThrowRange;
        discEffectZonePreview.transform.position = playerPosition + Vector3.up * 0.01f;

        UpdateNumberOfShownTrajectories(trajectoryParameters.Count);
    }

    public void UpdateThrowPreview(Dictionary<DiscScript, DiscTrajectoryParameters> trajectoryParameters)
    {
        UpdateTrajectoriesPreview(trajectoryParameters);
    }

    public void EndThrowPreview()
    {
        ClearHitByDiscTrajectories();

        UpdateNumberOfShownTrajectories(0);
        discEffectZonePreview.gameObject.SetActive(false);
    }
    #endregion

    #region Recall 

    public void StartRecallPreview(Dictionary<DiscScript, DiscTrajectoryParameters> trajectoryParameters, Vector3 recallPosition)
    {
        hitByDiscTrajectories = new Dictionary<DiscScript, Dictionary<GameObject, EnemyBase>>();

        discRecallRange = DiscManager.Instance.recallRange;
        discEffectZonePreview.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * discRecallRange;
        discEffectZonePreview.transform.position = recallPosition + Vector3.up * 0.01f;

        UpdateNumberOfShownTrajectories(trajectoryParameters.Count);
    }

    public void UpdateRecallPreview(Dictionary<DiscScript, DiscTrajectoryParameters> trajectoryParameters, Vector3 recallPosition)
    {
        discEffectZonePreview.transform.position = recallPosition + Vector3.up * 0.01f;

        if (trajectoryParameters.Count != currentNumberOfShownArrowPreviews)
            UpdateNumberOfShownTrajectories(trajectoryParameters.Count);

        UpdateTrajectoriesPreview(trajectoryParameters);
    }

    public void EndRecallPreview()
    {
        ClearHitByDiscTrajectories();

        discEffectZonePreview.SetActive(false);
        UpdateNumberOfShownTrajectories(0);
    }
    #endregion

    #region Special

    #region Teleportation
    public void StartTeleportationPreview(Vector3 startPos, Vector3 targetPos, bool canTeleport, CompetenceRecall currentRecallComp)
    {
        hitByDiscTrajectories = new Dictionary<DiscScript, Dictionary<GameObject, EnemyBase>>();

        movementGhostPreview.GetRenderer.material = canTeleport ? baseGhostMaterial : cantMoveThereMaterial;
        discEffectZonePreview.gameObject.SetActive(true);
        discEffectZonePreview.transform.localScale = Vector3.one * DiscManager.Instance.recallRange;
        discEffectZonePreview.transform.position = startPos + Vector3.up * 0.01f;

        movementGhostPreview.gameObject.SetActive(true);
        movementGhostPreview.transform.position = targetPos;

        Dictionary<DiscScript, DiscTrajectoryParameters> discsInNewPositionRangeParameters = DiscListingFactory.GetDiscInRangeTrajectory(targetPos, currentRecallComp);
        StartRecallPreview(discsInNewPositionRangeParameters, targetPos);

        foreach (EnemyBase enemy in EnemiesManager.Instance.GetAllInGameEnemiesOrdered)
        {
            enemy.DisplayAndActualisePreviewAttack(movementGhostPreview.transform);
        }

    }

    public void UpdateTeleportationPreview(Vector3 position, bool canTeleport, CompetenceRecall currentRecallComp)
    {
        movementGhostPreview.GetRenderer.material = canTeleport ? baseGhostMaterial : cantMoveThereMaterial;
        movementGhostPreview.transform.position = position;

        Dictionary<DiscScript, DiscTrajectoryParameters> discsInNewPositionRangeParameters = DiscListingFactory.GetDiscInRangeTrajectory(position, currentRecallComp);
        UpdateRecallPreview(discsInNewPositionRangeParameters, position);
    }

    public void EndTeleportationPreview()
    {
        ClearHitByDiscTrajectories();

        movementGhostPreview.gameObject.SetActive(false);

        EndRecallPreview();
        foreach (EnemyBase enemy in EnemiesManager.Instance.GetAllInGameEnemiesOrdered)
        {
            enemy.HidePreview(false);
        }
    }
    #endregion
    #endregion
}
