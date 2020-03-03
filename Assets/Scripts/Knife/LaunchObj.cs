using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchObj : MonoBehaviour
{
    Rigidbody myRigidBody;

    public float speed = 3;
    public float rotaSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //vitesse du couteau
        myRigidBody.AddForce(transform.forward * speed);

        //rotation du couteau
        //myRigidBody.AddTorque(transform.position / rotaSpeed, ForceMode.Force);
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            //ennemy
            case 10:
                //take damage
                break;

            default:
                AttachToObj(other.ClosestPointOnBounds(transform.position));
                break;
        }
    }


    void AttachToObj(Vector3 impactPoint)
    {
        myRigidBody.isKinematic = true;
        transform.position = impactPoint;
    }
}

