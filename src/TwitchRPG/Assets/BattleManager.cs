using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        MixerInteractive.Initialize(true);
        MixerInteractive.GoInteractive();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MixerInteractive.GetButton("attack"))
        {
            Debug.Log("Player Attacked");
        }
    }
}
