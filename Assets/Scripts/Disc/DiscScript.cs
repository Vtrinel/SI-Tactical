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

    public float speed = 3;
    public float rotaSpeed = 3;

    public bool isAttacking = false;
    public bool isInRange = true;

    Collider attachedObj;

    GameObject objLaunch;
    Vector3 destination;

    void Update()
    {
        if (isAttacking)
        {
            //vitesse du couteau
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            if(Vector3.Distance(destination, transform.position) < 0.1f)
            {
                isAttacking = false;
            }
        }

        myAnimator.SetBool("Forward", isAttacking);
        myAnimator.SetBool("InRange", isInRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("test");
        if (collision.gameObject == objLaunch || !isAttacking) { return; }

        DeamandeFx(collision.contacts[0].point);

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

        DeamandeFx(other.ClosestPointOnBounds(transform.position));

        switch (other.gameObject.layer)
        {
            //Player
            case 9:
                //recall or touch player
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

    void DeamandeFx(Vector3 collision)
    {
        GameObject newFx = FxManager.Instance.DemandeFx(FxManager.fxType.Hit);

        newFx.transform.position = collision;
        newFx.transform.rotation = Random.rotation;
    }


    void CollisionWithThisObj(Transform impactPoint)
    {
        myAnimator.SetTrigger("Collision");

        Debug.DrawRay(transform.position + transform.forward * .5f , Vector3.up, Color.red, 50);

        transform.position = transform.position + transform.forward * .5f;
    }

    public void AttackHere(Transform _objLaunch, Vector3 _destination)
    {
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        isAttacking = true;

        objLaunch = _objLaunch.gameObject;
        transform.position = new Vector3(_objLaunch.position.x, 0 , _objLaunch.position.z);
        destination = new Vector3(_destination.x, 0, _destination.z);

        //transform.LookAt(destination, Vector3.forward);
        //transform.position += transform.forward * 1.2f;
        transform.position = new Vector3(transform.position.x,  DiscManager.discHeight, transform.position.z);
        myRigidBody.isKinematic = false;
        myCollider.enabled = true;
    }

    public void RecallCrystal(Transform player)
    {
        if (isInRange)
        {
            AttackHere(transform, player.position);
        }
    }
}

