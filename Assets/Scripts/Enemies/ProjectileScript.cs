﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] Vector3 destination;
    [SerializeField] float speed = 3;
    [SerializeField] int damage = 1;
    [SerializeField] GameObject ObjLauncher;

    bool canMove = false;

    public void SetDestination(Vector3 _destination, GameObject _objLauncher)
    {
        destination = _destination;
        ObjLauncher = _objLauncher;
        canMove = true;

        transform.LookAt(destination);
    }

    
    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }   

        if(Vector3.Distance(transform.position, destination) < 0.001f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == ObjLauncher) { return; }

        switch (other.gameObject.layer)
        {
            //Player
            case 9:
                GameManager.Instance.GetPlayer.damageReceiptionSystem.ReceiveDamage(DamageTag.Enemy, damage);
                break;

            //ennemy
            case 10:
                other.GetComponent<DamageableEntity>().ReceiveDamage(DamageTag.Enemy, damage);
                break;

            default:

                break;
        }

        Destroy(gameObject);
    }
}
