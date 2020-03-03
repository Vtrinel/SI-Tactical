using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTestSam : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newCrystal = CrystalManager.Instance.GetCrystal();

            newCrystal.GetComponent<CrystalScript>().AttackHere(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            newCrystal.SetActive(true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach(GameObject crystal in CrystalManager.Instance.GetAllCrystalUse())
            {
                crystal.GetComponent<CrystalScript>().RecallCrystal(transform);
            }
        }
    }
}
