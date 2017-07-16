using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using EL.Dungeon;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

#if Unity_Editor
using UnityEditor;
#endif
public class DungeonGenerator : MonoBehaviour {

    public EL.Dungeon.DungeonData data;
    public int dungeonSet = 0;

    public int seed = 0;
    public bool randomizeSeedOnStart = true;
    public bool randomizeRoomSize = true;
    public DRandom random;

    public bool generationComplete = false;
    public int targetRooms = 10;
    public int roomsCount;
    public List<Room> rooms = new List<Room>();
    public List<Door> doors = new List<Door>();

    public float voxelPixelSize = 10f;

    public List<EL.Dungeon.Room> openSet = new List<EL.Dungeon.Room>();
    public Dictionary<Vector3, GameObject> globalVoxels = new Dictionary<Vector3, GameObject>();
    public List<GameObject> doorVoxelsTest = new List<GameObject>();

    public GameObject startRoom;
    public static int roomsCalledStart = 0;
    public float GenerationDelay = 0.5f;
    

    public bool generateWithTimer = true;
    [Tooltip("In miliseconds")] public int MaxGenerationTime = 1500;
    private Stopwatch stopwatch = new Stopwatch();

	void Start () {
        
        //instance = this;
        if (randomizeSeedOnStart) {
            seed = Random.Range(0, int.MaxValue);
        }

        random = new DRandom();
        random.Init(seed);

        if (randomizeRoomSize) {
            targetRooms = 15 + (int)(random.value() * 50f);
        }

        roomsCount = 0;
        globalVoxels = new Dictionary<Vector3, GameObject>();
        dungeonSet = random.range(0, data.sets.Count-1);
        //Debug.Log("Generating dungeon with data:");
        //Debug.Log("Rooms count: " + targetRooms);
        //Debug.Log("Using set: " + data.sets[dungeonSet].name);

        try
        {
	        StartCoroutine(StartGeneration());
	    }
	    catch (Exception e)
	    {
	        Console.WriteLine(e);
	        throw;
	    }

        Debug.Log( "Generated in: " + stopwatch.ElapsedMilliseconds + "ms");
    }
    private IEnumerator StartGeneration() {
        DDebugTimer.Start();
        stopwatch.Start();
        generationComplete = false;

        rooms = new List<Room>();
        doors = new List<Door>();

        int spawn = random.range(0, data.sets[dungeonSet].spawns.Count - 1);
        GameObject room = (GameObject)Instantiate(data.sets[dungeonSet].spawns[spawn].gameObject);
        startRoom = room;
        rooms.Add(room.GetComponent<Room>());
        room.transform.parent = this.gameObject.transform;
        room.transform.localPosition = Vector3.zero;
        openSet.Add(room.GetComponent<EL.Dungeon.Room>());
        room.GetComponent<Volume>().RecalculateBounds();
        AddGlobalVoxels(room.GetComponent<Volume>().voxels);
        roomsCount++;

        while (openSet.Count > 0 && (!generateWithTimer || stopwatch.ElapsedMilliseconds < MaxGenerationTime)) {
            stopwatch.Reset();

            GenerateNextRoom();
            yield return new WaitForSeconds(GenerationDelay);

            stopwatch.Start();
        }

        //process doors
        for (int i = 0; i < rooms.Count; i++) {
            for (int j = 0; j < rooms[i].doors.Count; j++) {
                if (rooms[i].doors[j].door == null && rooms[i].doors[j].sharedDoor != null) {
                    Door d = ((GameObject)Instantiate(data.sets[dungeonSet].doors[0].gameObject)).GetComponent<Door>();
                    doors.Add(d);
                    rooms[i].doors[j].door = d;
                    rooms[i].doors[j].sharedDoor.door = d;
                    //
                    d.gameObject.transform.position = rooms[i].doors[j].transform.position;
                    d.gameObject.transform.rotation = rooms[i].doors[j].transform.rotation;
                    d.gameObject.transform.parent = this.gameObject.transform;
                }
            }
        }
        //locked doors and keys, etc come next. 

        generationComplete = true;
        Debug.Log("DungeonGenerator::Generation completed : " + DDebugTimer.Lap() + "ms");

    }

    private void GenerateNextRoom() {
        Room lastRoom = startRoom.GetComponent<Room>();
        if (openSet.Count > 0) lastRoom = openSet[0];

        Debug.Log(lastRoom);
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = lastRoom.transform.position.x;
        cameraPosition.z = lastRoom.transform.position.z;
        Camera.main.transform.position = cameraPosition;

        //create a mutable list of all possible rooms
        List<Room> possibleRooms = new List<Room>();
        for (int i = 0; i < data.sets[dungeonSet].roomTemplates.Count; i++) {
            possibleRooms.Add(data.sets[dungeonSet].roomTemplates[i]);
        }
        possibleRooms.Shuffle(random.random);

        GameObject newRoom;
        GeneratorDoor door;
        bool roomIsGood = false;

        //Debug.Log("count: " + data.sets[dungeonSet].roomTemplates.Count);

        do
        {
            for (int i = 0; i < doorVoxelsTest.Count; i++)
            {
                GameObject.DestroyImmediate(doorVoxelsTest[i]);
            }
            doorVoxelsTest.Clear();

            if (roomsCount >= targetRooms)
            {
                possibleRooms = GetAllRoomsWithOneDoor(possibleRooms);
                //Debug.Log("ADDING END ROOMS TARGET REACHED!");
            }

            if (possibleRooms.Count == 0)
                Debug.Log("WHAT IS GOING ON!");

            int r = GetRoomNumberToTry(possibleRooms);
            GameObject roomToTry = possibleRooms[r].gameObject;
            possibleRooms.RemoveAt(r);

            newRoom = (GameObject)Instantiate(roomToTry);
            newRoom.transform.parent = this.gameObject.transform;
            door = ConnectRooms(lastRoom, newRoom.GetComponent<Room>());

            //room is now generated and in position... we need to test overlap now!
            Volume v = newRoom.GetComponent<Volume>();
            Room ro = newRoom.GetComponent<Room>();

            if (!CheckOverlap(door, v) && CheckSpace(newRoom, v, ro))
            {
                //Debug.Log("all next rooms will fit!");
                roomIsGood = true;
            }
            else
            {
                //Debug.Log(newRoom);
                GameObject.DestroyImmediate(newRoom);
                //Debug.Log("Try a different room!!!!--------");
                //destroy the room we just tried to place
            }
        } while (possibleRooms.Count > 0 && !roomIsGood && (!generateWithTimer || stopwatch.ElapsedMilliseconds < MaxGenerationTime));
        if (!roomIsGood) {
            //we failed!
            Debug.Log("NO ROoms THAT FIT, THIS IS BAAAAD! ... but should never happen!");
            
            //remove the door because its blocked (or something)
            lastRoom.doors.Remove(door);
            if (!lastRoom.hasOpenDoors())
                openSet.Remove(lastRoom);

        } else {
            GeneratorDoor otherDoor = newRoom.GetComponent<Room>().GetFirstOpenDoor();
            door.sharedDoor = otherDoor;
            otherDoor.sharedDoor = door;

            door.open = false;
            newRoom.GetComponent<Room>().GetFirstOpenDoor().open = false;

            rooms.Add(newRoom.GetComponent<Room>());

            AddGlobalVoxels(newRoom.GetComponent<Volume>().voxels);
            if (!lastRoom.hasOpenDoors()) openSet.Remove(lastRoom);
            if (newRoom.GetComponent<Room>().hasOpenDoors()) openSet.Add(newRoom.GetComponent<Room>());
            roomsCount++;

            //Debug.Log("Openset: " + openSet.Count);
        }       
    }

    private bool CheckSpace(GameObject newRoom, Volume v, Room ro)
    {
        bool hasSpace = true;

        for (int i = 0; i < ro.doors.Count; i++)
        {
            //we need to find the direction the door is pointing in world space..
            //Debug.Log(i + " : " + ro.doors[i].open);
            if (!ro.doors[i].open) continue; //check all OPEN doors BUT the one we're connecting with..
            if (ro.doors[i] == newRoom.GetComponent<Room>().GetFirstOpenDoor()) continue;
            //Debug.Log("Actually checking door: " + i);
            float rot = TransformUtils.NormalizeAngle(Mathf.RoundToInt(ro.doors[i].transform.rotation.eulerAngles.y));
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
            //Debug.Log("Drawing spheres");
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = ro.doors[i].voxelOwner.transform.position + (direction * Volume.VoxelScale);
            doorVoxelsTest.Add(g);

            if (globalVoxels.ContainsKey(TransformUtils.RoundVec3ToInt(ro.doors[i].voxelOwner.transform.position + (direction * Volume.VoxelScale))))
            {
                //we have a collision on the door neighbours
                //Debug.Log("WE HAVE A COLLISION WITH THE DOOR NEIGHBOURS");
                hasSpace = false;
                break;
            }
            else
            {
                //we good!
                //Debug.Log("We don't have a collision witht he door neighbours");
                //check doors against all other doors so that no door voxels overlap with other door  voxels
                for (int j = 0; j < openSet.Count; j++)
                {
                    for (int k = 0; k < openSet[j].doors.Count; k++)
                    {
                        if (!openSet[j].doors[k].open) continue;
                        float rot2 = TransformUtils.NormalizeAngle(Mathf.RoundToInt(openSet[j].doors[k].transform.rotation.eulerAngles.y));
                        Vector3 direction2 = new Vector3();
                        if (rot2 == 0)
                        {
                            ////Debug.Log("Door: " + i + " is facing: +X");
                            direction2 = new Vector3(1f, 0f, 0f);
                        }
                        else if (rot2 == 180)
                        {
                            ////Debug.Log("Door: " + i + " is facing: -X");
                            direction2 = new Vector3(-1f, 0f, 0f);
                        }
                        else if (rot2 == 90)
                        {
                            ////Debug.Log("Door: " + i + " is facing: -Z");
                            direction2 = new Vector3(0f, 0f, -1f);
                        }
                        else if (rot2 == 270)
                        {
                            ////Debug.Log("Door: " + i + " is facing: +Z");
                            direction2 = new Vector3(0f, 0f, 1f);
                        }

                        if (TransformUtils.RoundVec3ToInt(ro.doors[i].voxelOwner.transform.position + (direction * Volume.VoxelScale)) == TransformUtils.RoundVec3ToInt(openSet[j].doors[k].voxelOwner.transform.position + (direction2 * Volume.VoxelScale)))
                        {
                            hasSpace = false;
                            //Debug.Log("TWo door voxels overlapping!");
                            break;
                        }
                    }
                    if (!hasSpace) break;
                }
            }
        }

        return hasSpace;
    }

    private bool CheckOverlap(GeneratorDoor door, Volume v)
    {
        bool overlap = false;
        for (int i = 0; i < v.voxels.Count; i++)
        {
            //Check if voxel is there already
            if (globalVoxels.ContainsKey(TransformUtils.RoundVec3ToInt(v.voxels[i].gameObject.transform.position)))
            {
                //overlap found! bad!
                //Debug.Log("THERE IS AN OVERLAP!!");
                overlap = true;
                continue;
            }

            for (int j = 0; j < openSet.Count; j++)
            {
                for (int k = 0; k < openSet[j].doors.Count; k++)
                {
                    //check if door is in the globalVoxelList
                    if (!openSet[j].doors[k].open) continue;
                    //we also want to ignore the Door we're connecting with
                    if (openSet[j].doors[k] == door) continue;
                    float rot = TransformUtils.NormalizeAngle(Mathf.RoundToInt(openSet[j].doors[k].transform.rotation.eulerAngles.y));
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
                    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    g.transform.position = openSet[j].doors[k].voxelOwner.transform.position + (direction * Volume.VoxelScale);
                    g.GetComponent<Renderer>().material.color = Color.red;
                    doorVoxelsTest.Add(g);
                    if (TransformUtils.RoundVec3ToInt(v.voxels[i].gameObject.transform.position) == TransformUtils.RoundVec3ToInt(openSet[j].doors[k].voxelOwner.transform.position + (direction * Volume.VoxelScale)))
                    {
                        overlap = true;
                        //Debug.Log("Room is overlapping a door voxel neighbour!!!");
                    }
                    else
                    {
                        //Debug.Log("Room is NOT overlapping with a door voxel neighbour!");
                    }
                }
            }
        }

        return overlap;
    }

    private int GetRoomNumberToTry(List<Room> possibleRooms)
    {
        int doors = 0;
        int r = random.range(0, possibleRooms.Count - 1);
        ////Debug.Log("r: " + r);
        ////Debug.Log(possibleRooms.Count);
        GameObject roomToTry = possibleRooms[r].gameObject;
        doors = roomToTry.GetComponent<Room>().doors.Count;

        //If we picked a room with with one door, try again UNLESS we've have no other rooms to try
        if (doors == 1 && possibleRooms.Count > 1)
        {
            //Debug.Log("we're adding a room with one door when we have other's we could try first..");
            float chance = 1f - Mathf.Sqrt(((float)roomsCount / (float)targetRooms)); //the closer we are to target the less of a chance of changing rooms
            float randomValue = random.value();
            //Debug.Log("Chance: " + chance + " | Random value: " + randomValue);
            if (randomValue < chance)
            {
                r = random.range(0, possibleRooms.Count - 1);
                roomToTry = possibleRooms[r].gameObject;
                //Debug.Log("trying a new room");
                //Debug.Log("New room has doors: " + roomToTry.GetComponent<Room>().doors.Count);

                doors = roomToTry.GetComponent<Room>().doors.Count;
                if (doors == 1 && possibleRooms.Count > 1)
                {
                    float chance2 = 1f - Mathf.Sqrt(((float)roomsCount / (float)targetRooms)); //the closer we are to target the less of a chance of changing rooms
                    float randomValue2 = random.value();
                    if (randomValue2 < chance2)
                    {
                        r = random.range(0, possibleRooms.Count - 1);
                        roomToTry = possibleRooms[r].gameObject;
                    }
                    else
                    {
                        //Debug.Log("Oh well again..");
                    }

                }
            }
            else
            {
                //Debug.Log("Oh well!");
            }
        }

        return r;
    }

    private void AddGlobalVoxels(List<GameObject> voxels) {
        for (int i = 0; i < voxels.Count; i++) {
            ////Debug.Log(string.Format("Trying to add voxel {0} with key {1}", i, voxels[i].gameObject.transform.position));
            Vector3 position = TransformUtils.RoundVec3ToInt(voxels[i].gameObject.transform.position);
            if (globalVoxels.ContainsKey(position)) {
                //Debug.Log("Voxel we're trying to add to globalVoxels is already defined..");
            } else {
                globalVoxels.Add(position, voxels[i]);
            }
        }
    }


    public List<Room> GetAllRoomsWithOneDoor(List<Room> list) {
        //this could be cached at startup, doesn't have to be calculated every iteration, right?
        //Debug.Log("Rooms with one door only: ");

        List<Room> roomsWithOneDoor = new List<Room>();
        for (int i = 0; i < list.Count; i++) {
            if (list[i].doors.Count == 1) {
                roomsWithOneDoor.Add(list[i]);
                //Debug.Log("room : " + i);
            }
        }
        return roomsWithOneDoor;
    }

    public GeneratorDoor ConnectRooms(Room lastRoom, Room newRoom) {
        GeneratorDoor lastRoomDoor = lastRoom.GetRandomDoor(random); //this is the "EXIT" door of the last room, which we want to connect to a new room
        GeneratorDoor newRoomDoor = newRoom.GetFirstOpenDoor(); //we grab the first open door to allow rooms to have "flow";

        if (!lastRoomDoor) return null;

        newRoom.transform.rotation = Quaternion.AngleAxis((lastRoomDoor.transform.eulerAngles.y - newRoomDoor.transform.eulerAngles.y) + 180f, Vector3.up);
        Vector3 translate = lastRoomDoor.transform.position - newRoomDoor.transform.position;
        newRoom.transform.position += translate;
        newRoom.GetComponent<Volume>().RecalculateBounds();
        //calling this now to create a worldspace bounds based on the new position/rotation after alignment.
        //we will use this worldspace volume-grid later when making smarter dungeons that can not overlap.

        //we should replace oen of these doors so that
        //they both share the same instance... we don't need TWO doors at every doorway
        //we will remove one of the graphical door prefabs, but we should keep both Door gameobject/components
        //we don't want to set these until we actually commmit to placing this room (ie after volume checks)
        //lastRoomDoor.open = false;
        //newRoomDoor.open = false;
        return lastRoomDoor;
        //we return lastRoomDoor because we don't know what door it will grab, but we know newRoom will always grab firstOpenDoor()
    }

    public void Update() {
        //if (openSet.Count > 0) {
        //    if (timer <= 0) {
        //        if(generateWithTimer) GenerateNextRoom();
        //        timer = delayTime;
        //    } else {
        //        timer -= Time.deltaTime;
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    if(!generateWithTimer) GenerateNextRoom();
        //    //Debug.Log(roomsCount + " | " + roomsCalledStart);
        //}
        if (Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
  