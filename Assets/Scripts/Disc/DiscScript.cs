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
    [SerializeField] Collider myCollider = default;
    [SerializeField] Animator myAnimator = default;

    [Header("Damages")]
    [SerializeField] DamageTag damageTag = DamageTag.Player;
    [SerializeField] int currentDamagesAmount = 1;

    [Header("Movement")]
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float acceleration = 15f;
    float currentSpeed = 0f;

    List<Vector3> currentTrajectory = new List<Vector3>();
    public System.Action<DiscScript> OnReachedTrajectoryEnd = default;

    public bool isAttacking = false;
    public bool isInRange = true;
    bool retreivableByPlayer = false;

    //public float rotaSpeed = 3;


    Collider attachedObj;

    GameObject objLaunch;
    Vector3 destination;

    void Update()
    {
        if (currentTrajectory.Count > 0)
            UpdateTrajectory();

        myAnimator.SetBool("Forward", isAttacking);
        myAnimator.SetBool("InRange", isInRange);
    }

    #region Trajectory
    public void SetRetreivableByPlayer(bool retreivable)
    {
        retreivableByPlayer = retreivable;
    }

    public void StartTrajectory(DiscTrajectoryParameters newTrajectory)
    {
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        myRigidBody.isKinematic = false;

        isAttacking = true;

        currentTrajectory = newTrajectory.trajectoryPositions;
        currentSpeed = 0;

        transform.position = currentTrajectory[0];
        currentTrajectory.RemoveAt(0);
    }

    public void UpdateTrajectory()
    {
        if (currentTrajectory.Count == 0)
        {
            EndTrajectory();
            return;
        }

        if(currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }

        float remainingReachableDistance = currentSpeed * Time.deltaTime;
        bool reachedTrajectoryEnd = false;

        Vector3 currentPositionToNextPosition = currentTrajectory[0] - transform.position;
        float currentPositionToNextPositionDistance = currentPositionToNextPosition.magnitude;
        Vector3 totalMovement = Vector3.zero;
        int reachedTrajectoryPoints = 0;

        if(currentPositionToNextPositionDistance > remainingReachableDistance)
        {
            totalMovement += currentPositionToNextPosition.normalized * remainingReachableDistance;
        }
        else
        {
            while (remainingReachableDistance > 0)
            {
                totalMovement += currentPositionToNextPosition.normalized * Mathf.Clamp(currentPositionToNextPositionDistance, 0, remainingReachableDistance);
                remainingReachableDistance -= currentPositionToNextPositionDistance;
                if(remainingReachableDistance > 0)
                    reachedTrajectoryPoints++;

                if(reachedTrajectoryPoints == currentTrajectory.Count)
                {
                    reachedTrajectoryEnd = true;
                    break;
                }

                currentPositionToNextPosition = currentTrajectory[reachedTrajectoryPoints] - currentTrajectory[reachedTrajectoryPoints - 1];
                currentPositionToNextPositionDistance = currentPositionToNextPosition.magnitude;
            }
        }
        totalMovement.y = 0;
        transform.position += totalMovement;

        if(reachedTrajectoryPoints > 0)
        Debug.Log(reachedTrajectoryPoints);
        while (reachedTrajectoryPoints > 0)
        {
            currentTrajectory.RemoveAt(0);
            reachedTrajectoryPoints--;
        }

        if (reachedTrajectoryEnd)
            EndTrajectory();
    }

    public void InterruptTrajectory()
    {
        currentTrajectory = new List<Vector3>();
        isAttacking = false;
        SetRetreivableByPlayer(true);
    }

    public void EndTrajectory()
    {
        OnReachedTrajectoryEnd?.Invoke(this);
        isAttacking = false;
        SetRetreivableByPlayer(true);
    }
    #endregion

    #region Collisions and Interaction
    private void OnCollisionEnter(Collision collision)
    {
        print("test");
        if (collision.gameObject == objLaunch || !isAttacking) { return; }

        DemandeFx(collision.contacts[0].point);

        switch (collision.gameObject.layer)
        {
            default:
                CollisionWithThisObj(collision.gameObject.transform);
                isAttacking = false;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == objLaunch || !isAttacking) { return; }

        DemandeFx(other.ClosestPointOnBounds(transform.position));

        switch (other.gameObject.layer)
        {
            //Player
            case 9:
                //recall or touch player
                if (!retreivableByPlayer)
                    break;

                isAttacking = false;
                //DiscManager.Instance.DeleteCrystal(gameObject);
                DiscManager.Instance.PlayerRetreiveDisc(this);
                break;

            //ennemy
            case 10:
                //take damage
                //CollisionWithThisObj(other.transform);
                //attachedObj = other;
                //isAttacking = false;

                DamageableEntity hitDamageableEntity = other.GetComponent<DamageableEntity>();
                if (hitDamageableEntity != null)
                {
                    hitDamageableEntity.ReceiveDamage(damageTag, currentDamagesAmount);
                }
                break;

            default:
                CollisionWithThisObj(other.transform);
                attachedObj = other;
                isAttacking = false;
                break;
        }
    }
       
    void CollisionWithThisObj(Transform impactPoint)
    {
        myAnimator.SetTrigger("Collision");

        Debug.DrawRay(transform.position + transform.forward * .5f, Vector3.up, Color.red, 50);

        transform.position = transform.position + transform.forward * .5f;
    }
    #endregion

    #region 
    void DemandeFx(Vector3 collision)
    {
        GameObject newFx = FxManager.Instance.DemandeFx(FxManager.fxType.Hit);

        newFx.transform.position = collision;
        newFx.transform.rotation = Random.rotation;
    }
    #endregion
}

