using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscScript : MonoBehaviour
{
    [SerializeField] DiscType _discType = DiscType.Basic;
    public void SetDiscType(DiscType discType)
    {
        _discType = discType;
    }
    public DiscType GetDiscType => _discType;

    [Header("References")]
    [SerializeField] Rigidbody myRigidBody = default;
    [SerializeField] CapsuleCollider myCollider = default;
    public Vector3 GetColliderCenter => transform.position + myCollider.center;
    [SerializeField] Animator myAnimator = default;
    [SerializeField] KnockbackableEntity knockbackSystem = default;

    [Header("Damages")]
    [SerializeField] DamageTag damageTag = DamageTag.Player;
    [SerializeField] int currentDamagesAmount = 1;

    [Header("Movement")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float accelerationDuration = 0.2f;
    [SerializeField] AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    TimerSystem accelerationDurationSystem = new TimerSystem();

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
    [SerializeField] bool blockedByShields = true;
    [SerializeField] bool blockedByObstacles = true;
    public LayerMask GetTrajectoryCheckLayerMask => /*((blockedByEnemies ? 1 : 0) << 10)*/1 << 10 | ((blockedByShields ? 1 : 0) << 12) | ((blockedByObstacles ? 1 : 0) << 14);

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
    }

    private void OnDisable()
    {
        knockbackSystem.OnKnockbackUpdate -= MoveKnockback;
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
                HandleCollision(hit, direction);

                return true;
            }
            else
            {
                DamageableEntity hitDamageableEntity = hit.collider.GetComponent<DamageableEntity>();
                if (hitDamageableEntity != null)
                {
                    hitDamageableEntity.ReceiveDamage(damageTag, currentDamagesAmount);

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
            hitDamageableEntity.ReceiveDamage(damageTag, currentDamagesAmount);

            lastObjTouch = hitDamageableEntity.gameObject;
        }
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

    #region Collisions and Interaction - OLD
    private void OnTriggerEnter(Collider other)
    {
        return;

        if (other.gameObject == objLaunch || !isAttacking) { return; }

        switch (other.gameObject.layer)
        {
            //Player --> rappel géré dans le déplacement  
            case 9:
                if (!retreivableByPlayer)
                {
                    DemandeFx(other.ClosestPointOnBounds(transform.position));
                    break;
                }

                RetreiveByPlayer();                

                break;

            //ennemy --> dégâts gérés dans le déplacement
            case 10:
                DemandeFx(other.ClosestPointOnBounds(transform.position));

                DamageableEntity hitDamageableEntity = other.GetComponent<DamageableEntity>();
                if (hitDamageableEntity != null)
                {
                    hitDamageableEntity.ReceiveDamage(damageTag, currentDamagesAmount);

                    lastObjTouch = other.gameObject;
                }
                if (!blockedByShields)
                    break;

                break;

            //shield --> géré dans le déplacement
            case 12:
                if (!blockedByShields)
                    break;

                if (lastObjTouch == other.transform.parent.GetComponent<ShieldManager>().myObjParent) { return; } else
                {
                    DemandeFx(other.ClosestPointOnBounds(transform.position));
                    CollisionWithThisObj(other.transform);
                }

                break;

            //obstacle --> déplacement aussi
            case 14:
                if (!blockedByObstacles)
                    break;

                break;

            /*default:
                CollisionWithThisObj(other.transform);
                break;*/
        }
    }
       
    void CollisionWithThisObj(Transform impactPoint)
    {
        InterruptTrajectory();

        myAnimator.SetTrigger("Collision");
        Debug.DrawRay(transform.position + -transform.right * .5f, Vector3.up, Color.red, 50);

        transform.position = transform.position + -transform.right * .5f;
    }
    #endregion

    #region Feedbacks
    void DemandeFx(Vector3 collision)
    {
        GameObject newFx = FxManager.Instance.DemandeFx(FxManager.fxType.Hit);

        newFx.transform.position = collision;
        newFx.transform.rotation = Random.rotation;
    }
    #endregion
}

