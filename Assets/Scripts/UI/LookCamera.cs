using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var lookPos = Camera.main.transform.position - transform.position;
        lookPos.x = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
