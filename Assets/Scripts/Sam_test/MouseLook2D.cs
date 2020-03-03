using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook2D : MonoBehaviour
{
    public Vector3 rotaVector = new Vector3(1, 0, 0);

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + -90, rotaVector * Time.deltaTime);
    }
}
