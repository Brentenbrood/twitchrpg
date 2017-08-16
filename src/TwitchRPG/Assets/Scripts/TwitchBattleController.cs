using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class TwitchBattleController : MonoBehaviour, IResponseProcessor
{
    public BattlePanel battlepanel;
    public List<TwitchFighter> players = new List<TwitchFighter>();
    public GameObject humanPrefab;

    public TwitchBattleUI UI;

    IEnumerator Start()
    {
        //UI = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<TwitchOverworldUI>();
        //StartGettingPlayers();
        TwitchBotRequest playerRequest = JSAPI.GetAllPlayers();
        yield return playerRequest;
        PopulatePlayerList(playerRequest.Response);

        battlepanel.SetVisible(true);
    }

    void Update()
    {
        
    }

//    private void StartGettingPlayers()
//    {
//        //CurrentlyCollecting = true;
//        //UI.VotingPanel.StartVoting(possibleDoors.Keys.ToArray());
//
//        StartCoroutine(GetPlayers());
//
//    }

//    private IEnumerator GetPlayers()
//    {
//        float startTime = Time.time;
//        SocketConnection.Instance.AddResponder(this);
//        TwitchBotRequest botRequest = JSAPI.GetAllPlayers();
//        
//        while (startTime > Time.time)
//        {
//            yield return botRequest;
//        }
//
//        battlepanel.SetVisible(true);
//        SocketConnection.Instance.RemoveReponder(this);
//        yield return botRequest;
//    }

    public void PopulatePlayerList(JsonRequest json)
    {
        //Debug.Log(json.data["players"]);
        //SimpleJSON.JSONObject data = ;
        //Debug.Log(data);
        JSONObject players = json.data["players"].AsObject;
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
            JSONObject p = players[i].AsObject;
            PlayerData player = new PlayerData(p);
//            player.name = p["name"].Value;
//            player.attack = p["attack"].AsInt;
//            player.level = p["level"].AsInt;
//            player.xp = p["xp"].AsInt;
            Debug.Log(p["name"].Value);
            SpawnPlayer(player);
            //Debug.Log(playerData);
            // Process the player key and data as you need.
        }

    }

    public void SpawnPlayer(PlayerData player)
    {
        GameObject human = Instantiate(humanPrefab, transform.position, transform.rotation);
        TwitchFighter fighter = human.GetComponent<TwitchFighter>();
        fighter.Init(player);
        players.Add(fighter);
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
