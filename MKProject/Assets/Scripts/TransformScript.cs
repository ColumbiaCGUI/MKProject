using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformScript : MonoBehaviour {
	public Vector3 point;
	public Vector3 camDir; //Cameras position when you align
	public Vector3 camPos; // Cameras forward Position

	// Use this for initialization
	void Start () {
		Debug.Log (adjustCoordinateSpace (point));

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	Vector3 adjustCoordinateSpace(Vector3 point){
		Vector3 scale = new Vector3 (1, 1, 1);
		Vector3 viewDir = new Vector3 (camDir [0], 0, camDir [2]); //corrects for any accidental tilt in alignment
		Quaternion rot = Quaternion.LookRotation (viewDir, Vector3.up);
		Matrix4x4 m = Matrix4x4.identity;
		m.SetTRS (camPos, rot, scale);
		m = m.inverse;
		Vector3 newPoint = m.MultiplyPoint3x4 (point);
		return newPoint;
	}
}


