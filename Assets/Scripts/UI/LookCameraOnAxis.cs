using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCameraOnAxis : MonoBehaviour
{
    [SerializeField] Vector3 rota;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x * rota.x, transform.eulerAngles.y * rota.y, transform.eulerAngles.z * rota.z);
    }
}
