using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignBehavior : MonoBehaviour {
    public GameObject indicator;
    public Text posText;
    public Vector3 coordPosition;
    public Vector3 alignPosition;
    public Matrix4x4 worldToLocal;
    public Matrix4x4 localToWorld;


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        coordPosition = worldToLocal.MultiplyPoint3x4(Camera.main.transform.position);
        string displayTxt = "CurrentPos: " + coordPosition;
        posText.GetComponent<Text>().text = displayTxt;
    }
    public void aligned()
    {
        indicator.SetActive(false);
        localToWorld = createCoordSpace(Camera.main.transform.forward, Camera.main.transform.position);
        worldToLocal = localToWorld.inverse;
    }

    Matrix4x4 createCoordSpace(Vector3 camDir, Vector3 camPos)
    {
        Vector3 scale = new Vector3(1, 1, 1);
        Vector3 viewDir = new Vector3(camDir[0], 0, camDir[2]); //corrects for any accidental tilt in alignment
        Quaternion rot = Quaternion.LookRotation(viewDir, Vector3.up);
        Matrix4x4 m = Matrix4x4.identity;
        m.SetTRS(camPos, rot, scale);
        return m;

    }
}
