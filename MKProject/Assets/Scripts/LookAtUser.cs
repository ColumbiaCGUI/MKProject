using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 lookto = new Vector3(Camera.main.transform.position[0], 0, Camera.main.transform.position[2]);
        transform.LookAt(lookto);
        transform.RotateAround(Vector3.up, 110);
	}
}
