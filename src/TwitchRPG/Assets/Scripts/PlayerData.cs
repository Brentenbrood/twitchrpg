using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

public class PlayerData
{
    public string name;
    public int level;
    public int attack;
    public int xp;

    public PlayerData() { }

    public PlayerData(JSONObject data)
    {
        name = data["name"];
        level = data["level"];
        attack = data["attack"];
        xp = data["xp"];
    }
}
