using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwitchBattleController : MonoBehaviour, IResponseProcessor
{
    public bool CurrentlyCollecting;
    public float VotingTime = 15f;
    public float FetchTime = 3f;
    public BattlePanel battlepanel;
    public List<GameObject> players = new List<GameObject>();
    public GameObject humanPrefab;

    public TwitchOverworldUI UI;

    void Start()
    {
        UI = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<TwitchOverworldUI>();
        StartGettingPlayers();
    }

    void Update()
    {

        if (CurrentlyCollecting)
        {
            
        } else
        {
            
        }
    }

    private void StartGettingPlayers()
    {
        CurrentlyCollecting = true;
        //UI.VotingPanel.StartVoting(possibleDoors.Keys.ToArray());

        StartCoroutine(GetPlayers());
    }

    private IEnumerator GetPlayers()
    {
        float startTime = Time.time;
        SocketConnection.Instance.AddResponder(this);
        TwitchBotRequest botRequest = JSAPI.GetAllPlayers();
        
        CurrentlyCollecting = false;
        while (startTime + VotingTime > Time.time)
        {
            yield return botRequest;
        }

        CurrentlyCollecting = false;
        battlepanel.SetVisible(true);
        SocketConnection.Instance.RemoveReponder(this);
        yield return botRequest;
    }
    public void PopulatePlayerList(JsonRequest json)
    {
        //Debug.Log(json.data["players"]);
        //SimpleJSON.JSONObject data = ;
        //Debug.Log(data);
        var players = json.data["players"].AsObject;
        Debug.Log(players);
        /*foreach (SimpleJSON.JSONNode p in data)
        {
            Debug.Log(p);
            PlayerData player = new PlayerData();
            //player.name = p["name"].Value;
            //player.attack = p["attack"].AsInt;
            //player.level = p["level"].AsInt;
            //player.xp = p["xp"].AsInt;
            //Debug.Log(p["name"].Value);
            SpawnPlayer(player);
        }*/
        for (int i = 0; i < players.Count; i++)
        {
            string playerKey = (string)players[i];
            SimpleJSON.JSONObject p = (SimpleJSON.JSONObject)players[i];
            PlayerData player = new PlayerData();
            player.name = p["name"].Value;
            player.attack = p["attack"].AsInt;
            player.level = p["level"].AsInt;
            player.xp = p["xp"].AsInt;
            Debug.Log(p["name"].Value);
            SpawnPlayer(player);
            //Debug.Log(playerData);
            // Process the player key and data as you need.
        }

    }
    public void SpawnPlayer(PlayerData player)
    {
        //GameObject human = Instantiate(humanPrefab, transform.position, transform.rotation) as GameObject;
        //players.Add(human);
    }
    public string TypeName { get { return "playerlist"; } }

    public bool ProcessResponse(JsonRequest response)
    {
        PopulatePlayerList(response);
        /*if (response.data["direction"].IsString)
        {
            string dir = response.data["direction"];
            try
            {

            }
            catch (Exception)
            {
            }
        }*/

        return true;
    }
}
