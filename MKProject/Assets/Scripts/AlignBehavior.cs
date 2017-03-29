using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignBehavior : MonoBehaviour {
    public Material coolMat;
    public GameObject indicator;
    private Vector3 alignPosition;
    public Text posText;
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        string displayTxt = "CurrentPos: " + (Camera.main.transform.position - alignPosition);
        posText.text = displayTxt;
    }
    public void aligned()
    {
        indicator.GetComponent<Renderer>().material = coolMat;
        alignPosition = Camera.main.transform.position;


    }


}
