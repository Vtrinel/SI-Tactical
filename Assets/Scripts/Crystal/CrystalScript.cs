using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    [SerializeField] Rigidbody myRigidBody;
    [SerializeField] BoxCollider myBoxCollider;

    public float speed = 3;
    public float rotaSpeed = 3;

    bool isAttacking = false;

    Collider attachedObj;


    void FixedUpdate()
    {
        if (isAttacking)
        {
            //vitesse du couteau
            myRigidBody.AddForce(transform.forward * speed);
        }

        //rotation du couteau
        //myRigidBody.AddTorque(transform.position / rotaSpeed, ForceMode.Force);
    }


    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.layer);
        switch (other.gameObject.layer)
        {
            //Player
            case 9:
                //recall or touch player
                isAttacking = false;
                CrystalManager.Instance.DeleteCrystal(gameObject);
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
        myBoxCollider.enabled = false;
    }

    public void AttackHere(Transform objLaunch, Vector3 destination)
    {
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        isAttacking = true;

        transform.position = objLaunch.position;
        transform.LookAt(destination, Vector3.forward);
        transform.position += transform.forward * 1.2f;
        transform.position = new Vector3(transform.position.x,  CrystalManager.crystalHeight, transform.position.z);
        myRigidBody.isKinematic = false;
        myBoxCollider.enabled = true;
    }

    public void RecallCrystal(Transform player)
    {
        AttackHere(transform, player.position);
    }
}

