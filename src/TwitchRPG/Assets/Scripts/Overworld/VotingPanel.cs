using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dir = TwitchPlayerController.DoorDirection;

public class VotingPanel : MonoBehaviour, IResponseProcessor
{
    public GameObject DirectionLabelPrefab;

    Dictionary<Dir, int> votesDictionary = new Dictionary<Dir, int>();
    Dictionary<Dir, GameObject> labelDictionary = new Dictionary<Dir, GameObject>();

    private LinkedList<Dir> Changes = new LinkedList<Dir>();

	void Start ()
	{
	    SetVisible(false);

	}

    void Update()
    {
        foreach (Dir direction in Changes)
        {
            labelDictionary[direction].transform.Find("Count").GetComponent<Text>().text = votesDictionary[direction].ToString();
        }
        Changes.Clear();
    }

    public void StartVoting(Dir[] directions)
    {
        SetVisible(true);

        StartCoroutine(_startVoting(directions));
    }

    private IEnumerator _startVoting(Dir[] directions)
    {
        foreach (Dir direction in directions)
        {
            GameObject label = Instantiate(DirectionLabelPrefab, gameObject.transform);
            label.transform.Find("Command").GetComponent<Text>().text = "!" + direction.ToString().ToLower();
            labelDictionary.Add(direction, label);
            votesDictionary.Add(direction, 0);
        }

        SocketConnection.Instance.AddResponder(this);

        TwitchBotRequest botRequest = JSAPI.StartVoting();
        yield return botRequest;

    }

    public Dir EndVoting()
    {
        SocketConnection.Instance.RemoveReponder(this);
        StartCoroutine(_endVoting());

        //Get the most voted on direction
        Dir? bestdir = GetHighestVote();

        //clear all directions
        foreach (GameObject obj in labelDictionary.Values)
            Destroy(obj);

        votesDictionary.Clear();
        labelDictionary.Clear();

        return (Dir)bestdir;
    }

    private IEnumerator _endVoting()
    {
        TwitchBotRequest botRequest = JSAPI.EndVoting();
        yield return botRequest;

        SetVisible(false);
    }

    public bool IsDirectionAvailable(Dir direction)
    {
        return votesDictionary.ContainsKey(direction);
    }

    public void AddVote(Dir direction)
    {
        if (IsDirectionAvailable(direction))
        {
            votesDictionary[direction]++;
            UpdateLabel(direction);
        }
    }

    public void UpdateLabel(Dir direction)
    {
        Changes.AddLast(direction);
    }

    //TODO: Make this return an array
    public Dir GetHighestVote()
    {
        int highest = int.MinValue;
        Dir highDirection = Dir.FORWARD;
        foreach (var kvp in votesDictionary)
        {
            if (kvp.Value > highest)
            {
                highest = kvp.Value;
                highDirection = kvp.Key;
            }
        }

        return highDirection;
    }

    private void SetVisible(bool state)
    {
        GetComponent<Image>().enabled = state;
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(state);
        }
    }

    public string TypeName { get { return "AddVote"; } }
    public bool ProcessResponse(JsonRequest response)
    {
        if (response.data["direction"].IsString)
        {
            string dir = response.data["direction"];
            try
            {
                Dir realDirection = (Dir) Enum.Parse(typeof(Dir), dir, true);

                AddVote(realDirection);
            }
            catch (Exception )
            {
            }
        }

        return true;
    }
}
