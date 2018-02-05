using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataParser : MonoBehaviour {

    public SocketConnection socket;
    public List<PlayerData> players = new List<PlayerData>();
    public GameObject humanPrefab;
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PopulatePlayerList(SimpleJSON.JSONObject json)
    {
        Debug.Log(json);
        foreach (SimpleJSON.JSONNode p in json)
        {
            PlayerData player = new PlayerData();
            player.name = p["name"].Value;
            player.attack = p["attack"].AsInt;
            player.level = p["level"].AsInt;
            player.xp = p["xp"].AsInt;
            players.Add(player);
            Debug.Log(p["name"].Value);
            SpawnPlayer(player);
        }
    }
    public void SpawnPlayer(PlayerData player)
    {
        GameObject human = Instantiate(humanPrefab, transform.position, transform.rotation) as GameObject;
    }
}