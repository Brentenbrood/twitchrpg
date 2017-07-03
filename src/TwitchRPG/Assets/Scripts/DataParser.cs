using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataParser : MonoBehaviour {

    public string IP_Address = "";
    public int Port = 8124;

	// Use this for initialization
	void Start () {
        SocketConnection socket = new SocketConnection();
        //socket.SetupServer(IP_Address, Port);
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
