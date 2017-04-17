using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Initializer : MonoBehaviour
{
    public Text cnx;
    public NetworkManager NM;

	// Use this for initialization
	void Start ()
	{
#if !UNITY_EDITOR
        NM.StopClient();
        NM.StartClient();
#endif
        myStartClient();
        
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnApplicationQuit()
    {
        
    }

    public void myStartClient()
    {
        NM.StopClient();
        NM.StartClient();
        cnx.text = "Connect: " + NM.IsClientConnected();
        ;

    }
}
