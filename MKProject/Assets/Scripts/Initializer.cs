using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Initializer : MonoBehaviour
{
    //160.39.178.221 HL
    //160.39.166.238  Tango
    //Unknown message ID 1005 connId:1
    //UnityEngine.Networking.NetworkIdentity:UNetStaticUpdate()??

    public Text cnx;
    public NetworkManager NM;

	// Use this for initialization
	void Start ()
	{
#if !UNITY_EDITOR
      NM.StopHost();
      NM.StartHost();
#endif
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnApplicationQuit()
    {
        
    }

}
