using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwitchBattleUI : MonoBehaviour {
    public Canvas Canvas;
    public BattlePanel VotingPanel;

    void Start () {
        if(!Canvas) Canvas = GetComponent<Canvas>();

    }
}
