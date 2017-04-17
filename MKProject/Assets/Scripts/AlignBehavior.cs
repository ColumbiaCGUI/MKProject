using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignBehavior : MonoBehaviour {
    public Material coolMat;
    public GameObject indicator;
    public Vector3 alignPosition;
    public Text posText;
    public Vector3 coordPosition;
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        coordPosition = Camera.main.transform.position - alignPosition;
        string displayTxt = "CurrentPos: " + coordPosition;
        posText.text = displayTxt;
    }
    public void aligned()
    {
        indicator.GetComponent<Renderer>().material = coolMat;
        alignPosition = Camera.main.transform.position;
        indicator.SetActive(false);


    }


}
