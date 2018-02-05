using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

//TODO: Rename this to something appropriate
public static class JSAPI
{
    private static readonly SocketConnection Connection = SocketConnection.Instance;

    public static TwitchBotRequest StartVoting()
    {
        JsonRequest jsonRequest = new JsonRequest("StartVoting");
        TwitchBotRequest request = new TwitchBotRequest(jsonRequest);
        Connection.SendRequest(request);
        return request;
    }

    public static TwitchBotRequest EndVoting()
    {
        JsonRequest jsonRequest = new JsonRequest("EndVoting");
        TwitchBotRequest request = new TwitchBotRequest(jsonRequest);
        Connection.SendRequest(request);
        return request;
    }
    public static TwitchBotRequest GetAllPlayers()
    {
        JsonRequest jsonRequest = new JsonRequest("GetAllPlayers");
        TwitchBotRequest request = new TwitchBotRequest(jsonRequest);
        Connection.SendRequest(request);
        return request;
    }
}

public class TwitchBotRequest : CustomYieldInstruction
{
    public JsonRequest Request = null;
    public JsonRequest Response = null;

    public TwitchBotRequest()
    {
        
    }

    public TwitchBotRequest(JsonRequest request)
    {
        Request = request;
    }

    public TwitchBotRequest(string requestType, params JSONNode[] args)
    {
        Request = new JsonRequest(requestType);
        foreach (JSONNode node in args)
        {
            Request.data.Add(node);
        }
    }

    public override bool keepWaiting
    {
        get
        {
            return Response == null;
        }
    }
}
