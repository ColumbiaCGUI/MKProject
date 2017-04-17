using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignBehavior : MonoBehaviour {
    public GameObject indicator;
    public Text posText;
    public Vector3 coordPosition;
    public Vector3 alignPosition;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        coordPosition = Camera.main.transform.position - alignPosition;
        string displayTxt = "CurrentPos: " + coordPosition;
        posText.GetComponent<Text>().text = displayTxt;
    }
    public void aligned()
    {
        indicator.SetActive(false);
        alignPosition = Camera.main.transform.position;


    }


}
