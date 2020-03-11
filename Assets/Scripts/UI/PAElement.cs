using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAElement : MonoBehaviour
{
    [SerializeField] GameObject imageLife;

    public void SetValue(bool value)
    {
        imageLife.SetActive(value);
    }
}
