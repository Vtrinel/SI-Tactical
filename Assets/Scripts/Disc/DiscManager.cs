﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    public List<DiscScript> allDiscs = new List<DiscScript>();
    List<DiscScript> discsUse = new List<DiscScript>();

    public GameObject prefabDisc;

    public static float crystalHeight = 1f;

    public float rangeOgPlayer = 5;
    Transform player;


    private static DiscManager _instance;
    public static DiscManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer.transform;
    }

    private void Update()
    {
        foreach(DiscScript disc in discsUse)
        {
            disc.isInRange = (Vector3.Distance(player.position, disc.transform.position) < 7);
        }
    }

    public GameObject GetCrystal()
    {
        foreach (DiscScript element in allDiscs)
        {
            if (!element.gameObject.activeSelf && !discsUse.Contains(element))
            {
                discsUse.Add(element.GetComponent<DiscScript>());
                return element.gameObject;
            }
        }

        DiscScript newEnnemy = Instantiate(prefabDisc, transform).GetComponent<DiscScript>();
        allDiscs.Add(newEnnemy);
        discsUse.Add(newEnnemy);
        return newEnnemy.gameObject;
    }

    public void DeleteCrystal(GameObject element)
    {
        element.SetActive(false);
        discsUse.Remove(element.GetComponent<DiscScript>());
    }

    public List<DiscScript> GetAllCrystalUse()
    {
        return discsUse;
    }
}