using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedRotation : MonoBehaviour
{
    [SerializeField] Vector3 forcedRotation = Vector3.zero;
    void Update()
    {
        transform.rotation = Quaternion.Euler(forcedRotation);
    }
}
