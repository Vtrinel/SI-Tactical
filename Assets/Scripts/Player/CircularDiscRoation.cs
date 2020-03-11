using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularDiscRoation : MonoBehaviour
{
    [SerializeField] float rangeCircle = 2;

    List<int> discOnPlayer = new List<int>();

    [SerializeField] List<GameObject> miniDiscPrefab = new List<GameObject>();

    List<GameObject> currentDisc = new List<GameObject>();

    private void OnEnable()
    {
        DiscManager.Instance.OnDiscUpdate += SetDiscs;
    }

    void SetDiscs(Stack<DiscType> discList)
    {
        discOnPlayer.Clear();

        foreach (DiscType disc in discList)
        {
            if((int)disc == 0) { continue; }

            discOnPlayer.Add((int)disc);
        }

        UpdateDiscPos();
    }

    void UpdateDiscPos()
    {
        ClearAll();

        if(discOnPlayer.Count == 0) { return; }

        float angle = 360 / discOnPlayer.Count;
        int i = 1;

        foreach (int disc in discOnPlayer)
        {
            if(disc == 0) {
                print("none");
                continue; }

            GameObject newDisc = Instantiate(miniDiscPrefab[disc], transform);

            newDisc.transform.eulerAngles = new Vector3(0, (angle * i), 0);
            newDisc.transform.position += newDisc.transform.forward * rangeCircle;

            currentDisc.Add(newDisc);
            i++;
        }
    }

    void ClearAll()
    {
        foreach(GameObject disc in currentDisc)
        {
            Destroy(disc);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangeCircle);
    }
}
