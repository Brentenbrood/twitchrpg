using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwitchFighter : MonoBehaviour
{
    public string Name;
    public int Level;
    public int Attack; //TODO: @Brent waarvoor is dit?
    public int XP;

    public void Init(PlayerData playerData)
    {
        Name = playerData.name;
        Level = playerData.level;
        Attack = playerData.attack;
        XP = playerData.xp;
    }

    void Start () {
		
	}
	
	void Update () {
		
	}
}
