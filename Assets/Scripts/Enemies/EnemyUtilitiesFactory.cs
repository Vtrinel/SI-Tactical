using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyUtilitiesFactory
{
    public static Transform GetTheCloserObjOfMe(Transform me, List<Transform> allObj)
    {
        Dictionary<Transform, float> allObjAndDistance = new Dictionary<Transform, float>();

        foreach(Transform obj in allObj)
        {
            allObjAndDistance.Add(obj, Vector3.Distance(obj.position, me.position));
        }


        var keyAndValue = allObjAndDistance.OrderBy(kvp => kvp.Value).First();
        return keyAndValue.Key;
    }
}
