using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook2D : MonoBehaviour
{
    public Vector3 rotaVector = new Vector3(1, 0, 0);

    Transform player;

    private void Start()
    {
        player = GameManager.Instance.GetPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //float angle = Mathf.Atan2(player.position.y, player.position.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle + -90, rotaVector * Time.deltaTime);

        var lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
