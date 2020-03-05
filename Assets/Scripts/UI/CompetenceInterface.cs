using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceInterface : MonoBehaviour
{

    Canvas competenceCanvas;
    bool seeCanvas = false;

    private void Start()
    {
        competenceCanvas = GameObject.Find("CompetenceInterface").GetComponent<Canvas>  ();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (competenceCanvas)
                seeCanvas = !seeCanvas;
                competenceCanvas.gameObject.SetActive(seeCanvas);
        }
    }
}
