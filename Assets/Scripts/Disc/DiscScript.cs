using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscScript : MonoBehaviour
{
    [SerializeField] Rigidbody myRigidBody = default;
    [SerializeField] Collider myCollider = default;
    [SerializeField] Animator myAnimator = default;

    [Header("Damages")]
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


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == objLaunch) { return; }

        switch (other.gameObject.layer)
        {
            //Player
            case 9:
                //recall or touch player
                isAttacking = false;
                DiscManager.Instance.DeleteCrystal(gameObject);
                break;

            //ennemy
            case 10:
                //take damage
                //CollisionWithThisObj(other.transform);
                //attachedObj = other;
                //isAttacking = false;
                Debug.Log("Collision with enemy");

                DamageableEntity hitDamageableEntity = other.GetComponent<DamageableEntity>();
                if (hitDamageableEntity != null)
                {
                    hitDamageableEntity.LoseLife(currentDamagesAmount);
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

        print("vehez");
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
        transform.position = new Vector3(transform.position.x,  DiscManager.crystalHeight, transform.position.z);
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

