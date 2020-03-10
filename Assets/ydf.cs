using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ydf : MonoBehaviour
{
    public float range = 2;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
