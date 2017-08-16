using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Dir = TwitchPlayerController.DoorDirection;

//TODO: Make this a singleton
public class TwitchOverworldUI : MonoBehaviour
{
    public Canvas Canvas;
    public VotingPanel VotingPanel;

	void Start ()
	{
	    if(!Canvas) Canvas = GetComponent<Canvas>();
        
	}    
}
