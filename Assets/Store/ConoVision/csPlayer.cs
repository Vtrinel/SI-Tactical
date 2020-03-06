using UnityEngine;
using System.Collections;

public class csPlayer : MonoBehaviour {

	CharacterController cc;

	// Use this for initialization
	void Start () {

		cc = this.GetComponent<CharacterController>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetAxis("Vertical")!=0f){

			cc.SimpleMove(-transform.forward*Input.GetAxis("Vertical")*50f*Time.deltaTime);

		}

		if(Input.GetAxis("Horizontal")!=0f){

			transform.rotation*=Quaternion.Euler(Vector3.up*Input.GetAxis("Horizontal")*45f*Time.deltaTime);

		}
	
	}
}
