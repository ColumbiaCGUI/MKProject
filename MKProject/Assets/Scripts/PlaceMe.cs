using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMe : MonoBehaviour {

    public Vector3 localPos;
	// Use this for initialization
	void Start () {

        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void placeObject(Matrix4x4 toWorld)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = toWorld.MultiplyPoint3x4(localPos);
    }
}
