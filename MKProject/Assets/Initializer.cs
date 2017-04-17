using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Initializer : MonoBehaviour
{
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
