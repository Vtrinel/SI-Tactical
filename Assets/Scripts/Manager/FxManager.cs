using System.Collections;
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

    public void DemandeFx(FxType myTypeFx, Vector3 position)
    {
        if (CanCreateFx(myTypeFx))
        {
            GameObject newExplo = Instantiate(FxGameObjectStocked.fxGameObject, transform);
        }
    }

    bool CanCreateFx(FxType hisType)
    {
        bool canCreateFx = false;

        foreach(FxGameObject fxGameObject in FXGameObjectList)
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
}

public enum FxType
{
    Hit,
    Explosion,
}

[System.Serializable]
public class FxGameObject
{
    public FxType fxType;
    public GameObject fxGameObject;
}
