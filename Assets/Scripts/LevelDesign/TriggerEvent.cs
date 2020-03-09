using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] UnityEvent OnTriggerEnterEvent;
    [SerializeField] bool isInfinite = false;
    private bool playedOnce = false;
    private BoxCollider collider;

    private void OnDrawGizmos()
    {
        if (collider == null) collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.4f);
        Gizmos.DrawCube(transform.position + collider.center, collider.size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInfinite && playedOnce) return;

        switch (other.gameObject.layer)
        {
            //Player
            case 9:
                PlayEvent();
                break;

            //ennemy
            case 10:


            //shield
            case 12:

                break;

            default:
                
                break;
        }
    }

    void PlayEvent()
    {
        playedOnce = true;
        Debug.Log("Trigger Event On " + gameObject.name, this);
        OnTriggerEnterEvent?.Invoke();
    }
}
