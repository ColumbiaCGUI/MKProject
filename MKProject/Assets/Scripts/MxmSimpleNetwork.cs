using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using MercuryXM.Framework;

public class MxmSimpleNetwork : MxmBaseBehavior
{
    private Vector3 value;
    public Text otherPos;
    public GameObject tango;
    public GameObject AlignmentManager;

    // Use this for initialization
    void Start () {

        tango.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.touchCount == 1) sendCoordinates();

    }

    public void sendCoordinates()
    {
        InvokeRepeating("sendRoutine", 0.5f, 1.0f);
    }

    public void sendRoutine() {
            Debug.Log("sending");
            GameObject alignmentManager = GameObject.Find("AlignmentManager");
            AlignBehavior alignBehavior = alignmentManager.GetComponent<AlignBehavior>();
            value = alignBehavior.coordPosition;
            Debug.Log(value);
            GetNode().MxmInvoke(MxmMethod.Vector3, value, MxmControlBlockHelper.SelfDefaultTagAll);
    }

    public override void MxmInvoke(MxmMessageType msgType, Mxmessage message)
    {
        Debug.Log("here");
        var type = message.MxmMethod;
        switch (type)
        {
            case MxmMethod.Vector3:
                Debug.Log("Received Message");
                MxmessageVector3 msg = (MxmessageVector3)message;
                Debug.Log(msg.value);

                string displayTxt = "TangoPos: " + msg.value;
                otherPos.text = displayTxt;

                tango.SetActive(true);
                tango.transform.position = AlignmentManager.GetComponent<AlignBehavior>().localToWorld.MultiplyPoint3x4(msg.value);
                break;
            default:
                base.MxmInvoke(msgType, message);
                break;
        }
    }
}
