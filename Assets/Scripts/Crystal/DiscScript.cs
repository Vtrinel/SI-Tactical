using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscScript : MonoBehaviour
{
    [SerializeField] Rigidbody myRigidBody;
    [SerializeField] Collider myCollider;
    [SerializeField] Animator myAnimator;

    public float speed = 3;
    public float rotaSpeed = 3;

    bool isAttacking = false;

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
        }
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
                AttachToObj(other.ClosestPointOnBounds(transform.position));
                attachedObj = other;
                isAttacking = false;
                break;

            default:
                AttachToObj(other.ClosestPointOnBounds(transform.position));
                attachedObj = other;
                isAttacking = false;
                break;
        }
    }


    void AttachToObj(Vector3 impactPoint)
    {
        myRigidBody.isKinematic = true;
        transform.position = impactPoint;
        myCollider.enabled = false;
    }

    public void AttackHere(Transform _objLaunch, Vector3 _destination)
    {
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        isAttacking = true;

        objLaunch = _objLaunch.gameObject;
        transform.position = _objLaunch.position;
        destination = new Vector3(_destination.x, 0, _destination.z);

        print(destination);

        //transform.LookAt(destination, Vector3.forward);
        //transform.position += transform.forward * 1.2f;
        transform.position = new Vector3(transform.position.x,  DiscManager.crystalHeight, transform.position.z);
        myRigidBody.isKinematic = false;
        myCollider.enabled = true;
    }

    public void RecallCrystal(Transform player)
    {
        AttackHere(transform, player.position);
    }
}

