using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataParser : MonoBehaviour {

    public SocketConnection socket;
    public PlayerData players;
	// Use this for initialization
	void Start () {
        socket.GetAllPlayers();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
