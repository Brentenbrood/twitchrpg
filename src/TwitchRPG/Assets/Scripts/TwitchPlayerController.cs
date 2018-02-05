using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EL.Dungeon;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TwitchPlayerController : MonoBehaviour
{
    public enum DoorDirection
    {
        FORWARD,
        LEFT,
        RIGHT,
        BACKWARD
    }

    public GeneratorDoor TargetDoor;
    public bool CurrentlyVoting;
    public float VotingTime = 15f;

    public TwitchOverworldUI UI;

    private NavMeshAgent agent;
    private Dictionary<DoorDirection, GeneratorDoor> possibleDoors = null;

    void Start ()
	{
	    agent = GetComponent<NavMeshAgent>();
	    UI = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<TwitchOverworldUI>();
	}

    void Update ()
	{
	    if (!agent.isOnNavMesh)
	    {
	        agent.enabled = false;
	        agent.enabled = true;
            return;
	    }

	    if (CurrentlyVoting)
	        return;

	    float dist = agent.remainingDistance;
	    if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete &&
	        Math.Abs(agent.remainingDistance) < 0.001f)
	    {
            Debug.Log("Finished walking!");

            //if we have actually reached our goal
            if (Vector3.Distance(agent.pathEndPosition, TargetDoor.transform.position) < 0.5f)
            {
                SetNextDoor();
            }

            if(TargetDoor)
	            agent.SetDestination(TargetDoor.transform.position);
        }
    }

    private void SetNextDoor()
    {
        var otherDoor = TargetDoor.sharedDoor;
        Room otherRoom = otherDoor.GetComponentInParent<Room>();
        if (otherRoom.doors.Count == 1)
        {
            Debug.Log("Walking to the door behind me");
            TargetDoor = TargetDoor.sharedDoor;
        }
        else if (otherRoom.doors.Count == 2)
        {
            Debug.Log("Walking through the corridor");
            GeneratorDoor old = TargetDoor;
            TargetDoor = otherRoom.doors[0] == otherDoor ? otherRoom.doors[1] : otherRoom.doors[0];
            Debug.DrawLine(old.transform.position, TargetDoor.transform.position, Color.cyan, 0.1f, false);
        }
        else
        {
            Debug.Log("Starting a voting session");
            StartVotingSession();
        }
    }

    private void StartVotingSession()
    {
        CurrentlyVoting = true;

        possibleDoors = GetPossibleDirections();
        foreach (DoorDirection direction in possibleDoors.Keys)
        {
            Debug.Log(direction);
        }
        UI.VotingPanel.StartVoting(possibleDoors.Keys.ToArray());

        StartCoroutine(CollectVotes());
    }

    private IEnumerator CollectVotes()
    {
//        float startTime = Time.time;
//        while (startTime + VotingTime > Time.time)
//        {
//            yield return null;
//        }

        yield return new WaitForSeconds(VotingTime);

        Debug.Log("Collecting votes!");

        CurrentlyVoting = false;
        DoorDirection direction = UI.VotingPanel.EndVoting();
        TargetDoor = possibleDoors[direction];
    }

    #region Utility Function

    public Room GetAttachedRoom()
    {
        return TargetDoor ? TargetDoor.sharedDoor.GetComponentInParent<Room>() : null;
    }

    public Dictionary<DoorDirection, GeneratorDoor> GetPossibleDirections()
    {
        GeneratorDoor otherDoor = TargetDoor.sharedDoor;
        Room room = GetAttachedRoom();
        Volume volume = room.GetComponent<Volume>();
        volume.RecalculateBounds();

        Dictionary<DoorDirection, GeneratorDoor> possibleDirections = new Dictionary<DoorDirection, GeneratorDoor>();
        Vector3 roomCenter = volume.bounds.center;
        Vector3 rotationVector = GetDoorDirection(TargetDoor);
        foreach (GeneratorDoor door in room.doors)
        {
            if(door == otherDoor)
                continue;

            Vector3 dir = door.transform.position - roomCenter;

            float rot = Vector3.SignedAngle(rotationVector, dir, Vector3.up);
            Debug.Log(rot);
            //TODO: Get these angles from the edges of the bounds
            if (rot > -45 && rot < 45)
                possibleDirections.Add(DoorDirection.FORWARD, door);
            else if(rot >= -135 && rot <= -45)
                possibleDirections.Add(DoorDirection.LEFT, door);
            else if(rot >= 45 && rot <= 135)
                possibleDirections.Add(DoorDirection.RIGHT, door);
        }
        if (TargetDoor.GetComponentInParent<Room>().doors.Count > 1)
            possibleDirections.Add(DoorDirection.BACKWARD, otherDoor);

            return possibleDirections;
    }

    //TODO: Add to utility class
    private Vector2 TopDown(Vector3 pos)
    {
        return new Vector2(pos.x, pos.z);
    }

    private Vector3 GetDoorDirection(GeneratorDoor door)
    {
        float rot = TransformUtils.NormalizeAngle(Mathf.RoundToInt(door.transform.rotation.eulerAngles.y));

        Vector3 direction = new Vector3();
        if (rot == 0)
        {
            ////Debug.Log("Door: " + i + " is facing: +X");
            direction = new Vector3(1f, 0f, 0f);
        }
        else if (rot == 180)
        {
            ////Debug.Log("Door: " + i + " is facing: -X");
            direction = new Vector3(-1f, 0f, 0f);
        }
        else if (rot == 90)
        {
            ////Debug.Log("Door: " + i + " is facing: -Z");
            direction = new Vector3(0f, 0f, -1f);
        }
        else if (rot == 270)
        {
            ////Debug.Log("Door: " + i + " is facing: +Z");
            direction = new Vector3(0f, 0f, 1f);
        }

        return direction;
    }

    #endregion
}
