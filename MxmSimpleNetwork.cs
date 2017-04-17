using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MercuryXM.Framework;

public class MxmSimpleNetwork : MxmBaseBehavior
{
    private Vector3 value;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void sendCoordinates() { 
        if (Input.GetKeyDown(KeyCode.A) || Input.touchCount == 1)
        {
            GameObject alignmentManager = GameObject.Find("AlignmentManager");
            AlignBehavior alignBehavior = alignmentManager.GetComponent<AlignBehavior>();
            value = alignBehavior.coordPosition;
            Debug.Log(value);
            GetNode().MxmInvoke(MxmMethod.Vector3, value, MxmControlBlockHelper.SelfDefaultTagAll);
        } 
    }

    public override void MxmInvoke(MxmMessageType msgType, Mxmessage message)
    {
        var type = message.MxmMethod;
        switch (type)
        {
            case MxmMethod.Vector3:
                Debug.Log("Received Message");
                MxmessageVector3 msg = (MxmessageVector3)message;
                Debug.Log(msg.value);
                break;
            default:
                base.MxmInvoke(msgType, message);
                break;
        }
    }
}
