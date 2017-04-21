using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMoreStuff : MonoBehaviour
{
    public bool isShowing;
    private Vector3 realPos;
    private GameObject alignment;
    // Use this for initialization
    void Start()
    {
        isShowing = false;
        alignment = GameObject.Find("AlignmentManager");
        Vector3 relativePos = gameObject.transform.parent.gameObject.GetComponent<PlaceMe>().localPos;
        realPos = alignment.GetComponent<AlignBehavior>().localToWorld.MultiplyPoint3x4(relativePos);


    }

    // Update is called once per frame
    void Update()
    {
        if (!isShowing) gameObject.SetActive(false);
        Vector3 dist = realPos - Camera.main.transform.position;
        if (dist.magnitude > 3.0f) hide();


    }

    public void setVisible()
    {
        isShowing = true;
        gameObject.SetActive(true);
    }

    public void hide()
    {
        isShowing = false;
    }
}
