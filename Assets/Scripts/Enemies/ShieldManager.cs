using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    [SerializeField] GameObject FrontShield;
    [SerializeField] GameObject BackShield;
    [SerializeField] GameObject RightShield;
    [SerializeField] GameObject LeftShield;

    public AttackPos allShieldPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Flags]
    public enum AttackPos
    {
        None = 0,
        Front = 1,
        Back = 2,
        Right = 3,
        Lefty = 4
    }
}
