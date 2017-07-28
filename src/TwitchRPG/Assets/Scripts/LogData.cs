using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogResponse : IResponseProcessor {
    private readonly string typeName;

    public LogResponse(string typeName)
    {
        this.typeName = typeName;
    }

    public string TypeName
    {
        get { return typeName; }
    }

    public bool ProcessResponse(JsonRequest response)
    {
        Debug.Log(response.type + " : " + response.data.ToString());

        return true;
    }
}
