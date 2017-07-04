using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataParser : MonoBehaviour {

    public SocketIOComponent socket;

	// Use this for initialization
	void Start () {
        socket = this.GetComponent<SocketIOComponent>();
        socket.Emit("connected");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
