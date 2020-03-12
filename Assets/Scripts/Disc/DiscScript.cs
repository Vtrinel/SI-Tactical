using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscScript : MonoBehaviour
{
    [SerializeField] DiscType _discType = DiscType.Piercing;
    public void SetDiscType(DiscType discType)
    {
        _discType = discType;
    }
    public DiscType GetDiscType => _discType;

    [Header("References")]
    [SerializeField] Rigidbody myRigidBody = default;
    [SerializeField] CapsuleCollider myCollider = default;
    public Vector3 GetColliderCenter => transform.position + myCollider.center;
    public Vector3 GetColliderLocalCenter => myCollider.center;
    public float GetColliderRadius => myCollider.radius;
    [SerializeField] Animator myAnimator = default;
    [SerializeField] KnockbackableEntity knockbackSystem = default;

    [Header("Damages")]
    [SerializeField] DamageTag damageTag = DamageTag.Player;
    [SerializeField] int currentDamagesAmount = 1;
    public int GetCurrentDamage => currentDamagesAmount;

    [Header("Movement")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float accelerationDuration = 0.2f;
    [SerializeField] AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    TimerSystem accelerationDurationSystem = new TimerSystem();
    [SerializeField] EffectZoneType zoneType = EffectZoneType.DiscTrajectoryEndZone;

    float currentSpeed = 0f;
    List<Vector3> currentTrajectory = new List<Vector3>();
    public System.Action<DiscScript> OnReachedTrajectoryEnd = default;
    public System.Action<DiscScript> OnTrajectoryStopped = default;

    public bool isAttacking = false;
    bool isInRange = true;
    public bool IsInRange => isInRange;
    public void SetIsInRange(bool inRange)
    {
        isInRange = inRange;
    }

    bool retreivableByPlayer = false;
    bool isBeingRecalled = false;

    GameObject objLaunch;
    GameObject lastObjTouch;

    Vector3 destination;

    [Header("Modifiers")]
    [SerializeField] bool blockedByEnemies = false;
    public bool GetBlockedByEnemies => blockedByEnemies;
    [SerializeField] bool blockedByShields = true;
    [SerializeField] bool blockedByObstacles = true;
    public LayerMask GetTrajectoryCheckLayerMask => 1 << 10 | ((blockedByShields ? 1 : 0) << 12) | ((blockedByObstacles ? 1 : 0) << 14);
    [SerializeField] List<DiscModifier> discModifiers = new List<DiscModifier>();

    int numberOfStunedTurns = 0;

    EffectZoneType effectZoneToInstantiateOnHit = EffectZoneType.None;
    bool destroyOnHit = false;

    bool modifiersSetUp = false;
    public void SetUpModifiers()
    {
        if (modifiersSetUp)
            return;
        modifiersSetUp = true;

        numberOfStunedTurns = 0;
        effectZoneToInstantiateOnHit = EffectZoneType.None;
        destroyOnHit = false;

        foreach (DiscModifier modifier in discModifiers)
        {
            if(numberOfStunedTurns == 0)
            {
                DiscModifierStun stunModifier = modifier as DiscModifierStun;
                if (stunModifier != null)
                    numberOfStunedTurns = stunModifier.GetNumberOfStunedTurns;
            }

            if(effectZoneToInstantiateOnHit == EffectZoneType.None)
            {
                DiscModifierEffectZone effectZoneModifier = modifier as DiscModifierEffectZone;
                if (effectZoneModifier != null)
                {
                    effectZoneToInstantiateOnHit = effectZoneModifier.GetEffectZoneToCreateOnHit;
                    destroyOnHit = effectZoneModifier.GetDiscProjectileOnHit;
                }
            }
        }
    }

    private void Start()
    {
        tooltipCollider.SetTooltipInformations(TooltipInformationFactory.GetDiscTypeInformations(DiscManager.Instance.GetDiscInformations(_discType)));
        SetUpModifiers();
    }

    void Update()
    {
        if (currentTrajectory.Count > 0)
            UpdateTrajectory();

        myAnimator.SetBool("Forward", isAttacking);
        myAnimator.SetBool("InRange", isInRange);
    }

    private void OnEnable()
    {
        knockbackSystem.OnKnockbackUpdate += MoveKnockback;
        knockbackSystem.OnKnockbackEnded += GenerateKnockbackZoneOnEndTrajectory;

        if (tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip += StartHovering;
            tooltipCollider.OnEndTooltip += EndHovering;
        }
    }

    private void OnDisable()
    {
        knockbackSystem.OnKnockbackUpdate -= MoveKnockback;
        knockbackSystem.OnKnockbackEnded -= GenerateKnockbackZoneOnEndTrajectory;

        if (tooltipCollider != null)
        {
            tooltipCollider.OnStartTooltip -= StartHovering;
            tooltipCollider.OnEndTooltip -= EndHovering;
        }
    }

    #region Movement Check
    /// <summary>
    /// Returns true if something was hit, and therefore the movement needs to stop
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public bool TryToMoveFromTo(Vector3 startPosition, Vector3 endPos)
    {
        Vector3 direction = endPos - startPosition;
        float distance = direction.magnitude;
        direction.Normalize();

        startPosition += myCollider.center;
        endPos += myCollider.center;

        RaycastHit hit = new RaycastHit();
        if (Physics.SphereCast(startPosition, myCollider.radius, direction, out hit, distance, GetTrajectoryCheckLayerMask))
        {
            if (hit.collider.gameObject.layer != 10 || blockedByEnemies)
            {
                // test bouclier
                if (hit.collider.gameObject.layer == 12)
                {
                    Transform hitParent = hit.transform.parent;
                    if (hitParent != null)
                    {
                        ShieldManager objShielManager = hit.transform.parent.GetComponent<ShieldManager>();

                        if (objShielManager != null)
                        {
                            if (objShielManager.myObjParent == lastObjTouch)
                            {
                                return false;
                            }
                        }
                    }
                }

                HandleCollision(hit, direction);

                return true;
            }
            else
            {
                DamageableEntity hitDamageableEntity = hit.collider.GetComponent<DamageableEntity>();
                if (hitDamageableEntity != null)
                {
                    hitDamageableEntity.ReceiveDamage(damageTag, new DamagesParameters(currentDamagesAmount, numberOfStunedTurns));

                    lastObjTouch = hitDamageableEntity.gameObject;
                }
            }
        }

        return false;
    }

    public void MoveKnockback(Vector3 knockbackMove)
    {
        bool canMove = !TryToMoveFromTo(transform.position, transform.position + knockbackMove);
        if (canMove)
            transform.position += knockbackMove;
    }

    public void HandleCollision(RaycastHit hit, Vector3 initialMovementDirection)
    {
        InterruptTrajectory();
        DemandeFx(hit.point);

        if (hit.collider.gameObject.layer == 12)
        {
            SoundManager.Instance.PlaySound(Sound.ShieldGetHit, hit.transform.position);
        }
        else if (hit.collider.gameObject.layer == 14)
        {
            SoundManager.Instance.PlaySound(Sound.WallGetHit, hit.transform.position);
        }

        Vector3 horizontalNormal = hit.normal;
        horizontalNormal.y = 0;
        horizontalNormal.Normalize();

        // Place disc as close from the hit point as possible
        Vector3 newPos = hit.point + horizontalNormal * (myCollider.radius + 0.01f);
        newPos.y = 0;
        transform.position = newPos;

        // Gives the disc a knockback to simulate a rebound
        Vector3 reflectedDirection = Vector3.Reflect(initialMovementDirection, horizontalNormal);
        knockbackSystem.ReceiveKnockback(DamageTag.Environment, GetReboundOnObjectKnockback(), reflectedDirection);

        DamageableEntity hitDamageableEntity = hit.collider.GetComponent<DamageableEntity>();
        if (hitDamageableEntity != null)
        {
            hitDamageableEntity.ReceiveDamage(damageTag, new DamagesParameters(currentDamagesAmount, numberOfStunedTurns));

            lastObjTouch = hitDamageableEntity.gameObject;
            SoundManager.Instance.PlaySound(Sound.EnemyDamaged, hitDamageableEntity.transform.position);
        }

        if(effectZoneToInstantiateOnHit != EffectZoneType.None)
        {
            EffectZone newEffectZone = EffectZonesManager.Instance.GetEffectZoneFromPool(effectZoneToInstantiateOnHit);
            newEffectZone.StartZone(GetColliderCenter);

            if (destroyOnHit)
            {
                DiscManager.Instance.DestroyDisc(this);
            }
        }
    }

    public void GenerateKnockbackZoneOnEndTrajectory()
    {
        EffectZone newZone = EffectZonesManager.Instance.GetEffectZoneFromPool(zoneType);
        newZone.StartZone(GetColliderCenter);
    }
    #endregion

    #region Trajectory
    public void SetRetreivableByPlayer(bool retreivable)
    {
        retreivableByPlayer = retreivable;
    }
    public void SetIsBeingRecalled(bool recalled)
    {
        isBeingRecalled = recalled;
    }

    public void StartTrajectory(DiscTrajectoryParameters newTrajectory, GameObject _launcher)
    {
        objLaunch = _launcher;
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        myRigidBody.isKinematic = false;

        lastObjTouch = null;

        isAttacking = true;

        currentTrajectory = newTrajectory.trajectoryPositions;
        currentSpeed = 0;

        transform.position = currentTrajectory[0];
        currentTrajectory.RemoveAt(0);

        accelerationDurationSystem.ChangeTimerValue(accelerationDuration);
        accelerationDurationSystem.StartTimer();
    }

    public void UpdateTrajectory()
    {
        if (currentTrajectory.Count == 0)
        {
            EndTrajectory(true);
            return;
        }

        bool canMove = true;
        Vector3 currentStartPosition = transform.position;
        Vector3 currentEndPosition = transform.position;

        if (!accelerationDurationSystem.TimerOver)
        {
            accelerationDurationSystem.UpdateTimer();
            currentSpeed = accelerationCurve.Evaluate(accelerationDurationSystem.GetTimerCoefficient) * maxSpeed;
        }

        float remainingReachableDistance = currentSpeed * Time.deltaTime;
        bool reachedTrajectoryEnd = false;

        Vector3 currentPositionToNextPosition = currentTrajectory[0] - transform.position;
        float currentPositionToNextPositionDistance = currentPositionToNextPosition.magnitude;
        Vector3 totalMovement = Vector3.zero;
        Vector3 lastMovementDirection = Vector3.forward;
        int reachedTrajectoryPoints = 0;

        if (currentPositionToNextPositionDistance > remainingReachableDistance)
        {
            Vector3 movement = currentPositionToNextPosition.normalized * remainingReachableDistance;
            currentEndPosition = currentStartPosition + movement;
            canMove = !TryToMoveFromTo(currentStartPosition, currentEndPosition);

            totalMovement += movement;
            lastMovementDirection = totalMovement.normalized;
        }
        else
        {
            while (remainingReachableDistance > 0)
            {
                Vector3 newMovement = currentPositionToNextPosition.normalized * Mathf.Clamp(currentPositionToNextPositionDistance, 0, remainingReachableDistance);

                currentStartPosition = transform.position + totalMovement;
                currentEndPosition = currentStartPosition + newMovement;

                if (TryToMoveFromTo(currentStartPosition, currentEndPosition))
                {
                    canMove = false;
                    break;
                }

                totalMovement += newMovement;

                lastMovementDirection = newMovement.normalized;

                remainingReachableDistance -= currentPositionToNextPositionDistance;
                if (remainingReachableDistance > 0)
                    reachedTrajectoryPoints++;

                if (reachedTrajectoryPoints == currentTrajectory.Count)
                {
                    reachedTrajectoryEnd = true;
                    break;
                }

                currentPositionToNextPosition = currentTrajectory[reachedTrajectoryPoints] - currentTrajectory[reachedTrajectoryPoints - 1];
                currentPositionToNextPositionDistance = currentPositionToNextPosition.magnitude;
            }
        }

        if (canMove)
        {
            totalMovement.y = 0;
            transform.position += totalMovement;

            if (reachedTrajectoryPoints > 0)
            {
                while (reachedTrajectoryPoints > 0)
                {
                    currentTrajectory.RemoveAt(0);
                    reachedTrajectoryPoints--;
                }
            }

            if (lastMovementDirection != Vector3.zero)
            {
                float rotY = Mathf.Atan2(lastMovementDirection.x, lastMovementDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, rotY, 0);
            }
        }

        if (reachedTrajectoryEnd)
            EndTrajectory(true);
    }

    public void InterruptTrajectory()
    {
        OnTrajectoryStopped?.Invoke(this);

        OnTrajectoryStopped = null;
        OnReachedTrajectoryEnd = null;

        currentTrajectory = new List<Vector3>();
        isAttacking = false;
        SetRetreivableByPlayer(true);
    }

    public void EndTrajectory(bool checkIfRetreive)
    {
        OnTrajectoryStopped?.Invoke(this);
        OnReachedTrajectoryEnd?.Invoke(this);

        OnTrajectoryStopped = null;
        OnReachedTrajectoryEnd = null;

        isAttacking = false;
        SetRetreivableByPlayer(true);

        GenerateKnockbackZoneOnEndTrajectory();

        if (isBeingRecalled && checkIfRetreive)
            RetreiveByPlayer();

    }
    #endregion

    #region Collision responses
    public void RetreiveByPlayer()
    {
        isAttacking = false;
        EndTrajectory(false);
        OnReachedTrajectoryEnd = null;
        DiscManager.Instance.PlayerRetreiveDisc(this);
    }

    public void ReboundOnObject(Vector3 reflectedDirection)
    {
        knockbackSystem.ReceiveKnockback(DamageTag.Environment, GetReboundOnObjectKnockback(), reflectedDirection);
    }

    public KnockbackParameters GetReboundOnObjectKnockback()
    {
        KnockbackParameters parameters = new KnockbackParameters();

        parameters.knockbackForce = currentSpeed / 4f;
        parameters.knockbackDuration = 0.02f;
        parameters.knockbackAttenuationDuration = 0.08f;
        parameters.canKnockbackDiscs = true;

        return parameters;
    }
    #endregion

    #region Feedbacks
    void DemandeFx(Vector3 collision)
    {
        FxManager.Instance.DemandeFx(FxType.Hit, collision);

        //newFx.transform.position = collision;
        //newFx.transform.rotation = Random.rotation;
    }
    #endregion

    [Header("Tooltips")]
    [SerializeField] TooltipCollider tooltipCollider = default;

    public void StartHovering()
    {
        //Debug.Log("Start Hovering " + name);
    }

    public void EndHovering()
    {
        //Debug.Log("End Hovering " + name);
    }
}

