using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class JsonRequest
{
    public string type = String.Empty;
    public bool request = true;
    public JSONNode data = new JSONObject();

    public JsonRequest()
    {
        
    }

    public JsonRequest(string type, JSONNode data = null, bool request = true)
    {
        this.type = type;
        this.data = data ?? new JSONObject();
        this.request = request;
    }

    public byte[] GetBytes()
    {
        JSONObject requestRoot = new JSONObject();
        requestRoot.Add("type", type);
        requestRoot.Add("request", request);
        requestRoot.Add("data", data);

        return System.Text.Encoding.UTF8.GetBytes(requestRoot.ToString());
    }
}
