using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    [HideInInspector] public GameObject myObjParent;

    [SerializeField] GameObject FrontShield;
    [SerializeField] GameObject BackShield;
    [SerializeField] GameObject RightShield;
    [SerializeField] GameObject LeftShield;

    [SerializeField] ShieldPos allShieldPos;

    // Start is called before the first frame update
    void Start()
    {
        SpawnShield();
    }

    void SpawnShield()
    {
        FrontShield.SetActive(allShieldPos.HasFlag(ShieldPos.Front));
        BackShield.SetActive(allShieldPos.HasFlag(ShieldPos.Back));
        RightShield.SetActive(allShieldPos.HasFlag(ShieldPos.Right));
        LeftShield.SetActive(allShieldPos.HasFlag(ShieldPos.Left));
    }

    [Flags]
    public enum ShieldPos
    {
        Front = (1 << 0),
        Back = (1 << 1),
        Right = (1 << 2),
        Left = (1 << 3),
    }
}
