using System;
using System.Collections;
using System.Collections.Generic;
using EL.Dungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldController : MonoBehaviour
{
#region Singleton
    private static OverworldController instance;

    public static OverworldController Get
    {
        private set
        {
            if(instance)
                throw new Exception("instance of OverworldController is already assigned");

            instance = value;
        }
        get { return instance; }
    }
#endregion

    private Vector3 playerLocation;
    private Quaternion playerRotation;
    private GeneratorDoor targetDoor;

    public bool SavedLocation = false;

    // Use this for initialization
    void Awake () {
        DontDestroyOnLoad(gameObject);

        Get = this;
    }

    public void SwitchToBattle(TwitchPlayerController player)
    {
        SavePlayerLocation(player);
    }

    private void SavePlayerLocation(TwitchPlayerController player)
    {
        playerLocation = player.gameObject.transform.position;
        playerRotation = player.gameObject.transform.rotation;
        targetDoor = player.TargetDoor;
    }

    // Update is called once per frame
//    void Update () {
//		
//	}
}
