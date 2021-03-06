﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    #region Singleton
    private static FxManager _instance;
    public static FxManager Instance { get { return _instance; } }

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
    #endregion

    [Header("List FX gameobject")]
    //[Tooltip("Don't forget to put the FX auto remove script on the fx game object")]
    public FxGameObject[] FXGameObjectList;

    private FxGameObject FxGameObjectStocked = default;

    public void CreateFx(FxType myTypeFx, Vector3 position)
    {
        FxGameObject fxGameObject = GetFxGameObject(myTypeFx);
        if (fxGameObject != null)
        {
            if (fxGameObject.fxGameObject == null)
                return;

            GameObject newFx = Instantiate(fxGameObject.fxGameObject);
            newFx.transform.position = position + fxGameObject.offset;
        }
    }

    public void CreateFxThrow(FxType myTypeFx, Vector3 position, Quaternion roration)
    {
        FxGameObject fxGameObject = GetFxGameObject(myTypeFx);
        if (fxGameObject != null)
        {
            GameObject newFx = Instantiate(fxGameObject.fxGameObject);
            newFx.transform.position = position + fxGameObject.offset;

            if (myTypeFx == FxType.discThrow)
            {
                // Change the rotation
                newFx.transform.position = position + fxGameObject.offset;
                var modifiedRotation = roration.eulerAngles;
                modifiedRotation = new Vector3(modifiedRotation.x, modifiedRotation.y - 90, modifiedRotation.z);

                newFx.transform.rotation = Quaternion.Euler(modifiedRotation);
            }
        }
    }

    public GameObject SendFx(FxType myTypeFx, Vector3 position)
    {
        FxGameObject fxGameObject = GetFxGameObject(myTypeFx);
        if (fxGameObject != null)
        {
            GameObject newFx = Instantiate(fxGameObject.fxGameObject);
            newFx.transform.position = position + fxGameObject.offset;

            return newFx;
        }
        return null;
    }

    bool CanCreateFx(FxType hisType)
    {
        bool canCreateFx = false;

        foreach (FxGameObject fxGameObject in FXGameObjectList)
        {
            // Check if tag name exist
            if (fxGameObject.fxType == hisType)
            {
                canCreateFx = true;

                // Check if gameobject exist
                if (fxGameObject.fxGameObject == null)
                {
                    Debug.LogWarning("Can't find FX object !");
                    return false;
                }
                else
                {
                    FxGameObjectStocked = fxGameObject;
                }
            }
        }
        return canCreateFx;
    }

    FxGameObject GetFxGameObject(FxType type)
    {
        FxGameObject fxGameObject = null;

        for (int i = 0; i < FXGameObjectList.Length; i++)
        {
            if(FXGameObjectList[i].fxType == type)
            {
                fxGameObject = FXGameObjectList[i];
                return fxGameObject;
            }
        }

        return fxGameObject;
    }
}

public enum FxType
{
    // Disc
    discThrow,  //1
    discRecall,
    discTrail,
    genericImpact,
    enterRecallZone,
    leaveRecallZone,
    discDestroyed,  //7

    // Enemy
    enemyDamage,    //8
    enemySpawn,
    enemySpawnPreparation,
    enemyDeath,
    enemyProjectileFire,
    enemyProjectileTrail,
    enemyImpactShield,  //14

    // Player
    playerGetHit,     // 15
    playerShockwave,
    playerTeleport,
    playerDeath,
    playerGhost,    // 19

    // Competence
    competenceExplosion,    //20

    // Environmenent
    statueExplosion,    //21
    totemUsable,    //22

    none,
    discExplosion,
}

[System.Serializable]
public class FxGameObject
{
    public FxType fxType;
    public GameObject fxGameObject;
    public Vector3 offset;
}
