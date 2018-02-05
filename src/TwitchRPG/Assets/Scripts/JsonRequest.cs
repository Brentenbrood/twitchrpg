using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class JsonRequest
{
    public string type
    {
        set { internalJson["type"] = value; }
        get { return internalJson["type"]; }
    }

    public bool request
    {
        set { internalJson["request"] = value; }
        get { return internalJson["request"]; }
    }
    public bool IsRequest { get { return request; } }

    public JSONNode data
    {
        set { internalJson["data"] = value; }
        get { return internalJson["data"]; }
    }

    private JSONObject internalJson = new JSONObject();

    public JsonRequest(string type, JSONNode data = null, bool request = true)
    {
        this.type = type;
        this.data = data ?? new JSONObject();
        this.request = request;
    }

    public JsonRequest(JSONNode jsonNode)
    {
        //validate the json
        if (!ValidateJsonForRequest(jsonNode))
            throw new Exception("jsonObject is not strictly a JsonRequest");

        internalJson = jsonNode.AsObject;
    }

    public JsonRequest(JSONObject jsonObject)
    {
        //validate the json
        if(!ValidateJsonForRequest(jsonObject))
            throw new Exception("jsonObject is not strictly a JsonRequest");

        internalJson = jsonObject;
    }

    public static bool ValidateJsonForRequest(JSONNode jsonNode)
    {
        if (!jsonNode.IsObject) return false;
        JSONObject json = jsonNode.AsObject;

        if (!json["type"].IsString) return false;

        Dictionary<string, JSONNodeType> allowedFields = new Dictionary<string, JSONNodeType>()
        {
            {"type", JSONNodeType.String},
            {"request", JSONNodeType.Boolean },
            {"data", JSONNodeType.Object },
            {"data", JSONNodeType.Array }
        };

        foreach (KeyValuePair<string, JSONNode> kvp in json)
        {
            if (!isValidField(kvp, allowedFields))
                return false;
        }

        return true;
    }

    private static bool isValidField(KeyValuePair<string, JSONNode> kvp, Dictionary<string, JSONNodeType> allowedFields)
    {
        foreach (KeyValuePair<string, JSONNodeType> allowedField in allowedFields)
        {
            if (kvp.Key == allowedField.Key && kvp.Value.Tag == allowedField.Value)
                return true;
        }

        return false;
    }

    public byte[] GetBytes()
    {
        return System.Text.Encoding.UTF8.GetBytes(internalJson.ToString());
    }
}
