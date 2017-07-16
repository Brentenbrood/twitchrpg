using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EL.Dungeon;
using UnityEditor;
using UnityEngine;

//By Maiko Steeman
[CustomEditor(typeof(DungeonSetBuilder))]
[CanEditMultipleObjects]
public class DungeonSetBuilderEditor : Editor
{
    private SerializedProperty set;
    private SerializedProperty ignoreList;

    private SerializedProperty setName;
    public string SetName
    {
        get
        {
            if (string.IsNullOrEmpty(setName.stringValue))
                return SetBuilder.name;
            return setName.stringValue;
        }
    }

    private DungeonSetBuilder dungeonSetBuilder;
    public DungeonSetBuilder SetBuilder
    {
        get
        {
            if (!dungeonSetBuilder)
                dungeonSetBuilder = (DungeonSetBuilder) target;
            return dungeonSetBuilder;
        }
        set { dungeonSetBuilder = value; }
    }

    void OnEnable()
    {
        set = serializedObject.FindProperty("Set");
        ignoreList = serializedObject.FindProperty("IgnoreList");

        setName = serializedObject.FindProperty("SetName");
        setName.stringValue = SetBuilder.name;

        CreateBaseFolderStructure();

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(setName);
        EditorGUILayout.PropertyField(set);
        EditorGUILayout.Separator();

#pragma warning disable CS0618 // Type or member is obsolete
        EditorGUIUtility.LookLikeInspector();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(ignoreList, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
        EditorGUIUtility.LookLikeControls();
#pragma warning restore CS0618 // Type or member is obsolete


        string btntxt = "Update DungeonSet Asset";
        if (!set.objectReferenceValue)
            btntxt = "Create DungeonSet Asset";

        if (GUILayout.Button(btntxt))
        {
            string sn = SetName;

            Debug.Log("Starting to create asset: '" + sn + ".asset'");

            //if (!AssetDatabase.IsValidFolder("Assets/Dungeons/" + sn))
            //    AssetDatabase.CreateFolder("Assets/Dungeons", sn);
            //if (!AssetDatabase.IsValidFolder("Assets/Dungeons/" + sn + "/Templates"))
            //    AssetDatabase.CreateFolder("Assets/Dungeons/" + sn, "Templates");
            CreateFolderTo("Assets/Dungeons/" + sn + "/Templates");

            DungeonSet dungeonSet = AssetDatabase.LoadAssetAtPath<DungeonSet>("Assets/Dungeons/" + sn + "/" + sn + ".asset");
            if (!dungeonSet)
            {
                dungeonSet = ScriptableObject.CreateInstance<DungeonSet>();
                dungeonSet.name = sn;
                AssetDatabase.CreateAsset(dungeonSet, "Assets/Dungeons/" + sn + "/" + sn + ".asset");
                Debug.Log("Creating dungeonset asset...");
            }

            GameObject[] ignores = SetBuilder.IgnoreList;
            List<Room> rooms = new List<Room>(SetBuilder.transform.childCount);
            List<Door> doors = new List<Door>();

            foreach (Transform transform in SetBuilder.transform)
            {
                if (ignores.Contains(transform.gameObject) ||
                    (!transform.GetComponent<Room>() && !transform.GetComponent<Door>())) continue;

                GameObject prefab = PrefabUtility.CreatePrefab(
                    "Assets/Dungeons/" + sn + "/Templates/" + transform.name + ".prefab",
                    transform.gameObject,
                    PrefabUtility.GetPrefabType(transform.gameObject) == PrefabType.PrefabInstance ? ReplacePrefabOptions.ConnectToPrefab : ReplacePrefabOptions.Default);
                FinaliseRoomPrefab(prefab);

                try
                {
                    Room room = prefab.GetComponent<Room>();
                    if (room)
                    {
                        CheckRoom(room);
                        room.GetComponent<Volume>().RecalculateBounds();
                        rooms.Add(room);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                Door door = prefab.GetComponent<Door>();
                if (door)
                    doors.Add(door);
            }


            dungeonSet.roomTemplates = rooms;
            dungeonSet.doors = doors;

            AssetDatabase.SaveAssets();

            set.objectReferenceValue = dungeonSet;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = dungeonSet;

            Debug.Log("Completed creating asset: '" + sn + ".asset'");
        }
        
    }

    private void FinaliseRoomPrefab(GameObject prefab)
    {
        prefab.transform.rotation = Quaternion.identity;
        
    }

    //TODO: Should I automatically try to fix these errors?
    private void CheckDoor(GeneratorDoor door)
    {
        //check if VoxelOwner is assigned
        if(!door.voxelOwner)
            throw new Exception(door.GetPath() + ": has an empty voxelOwner variable");
    }

    //TODO: Should I automatically try to fix these errors?
    private void CheckRoom(Room room)
    {
        //Does the room have a volume?
        Volume volume = room.GetComponent<Volume>();
        if (!volume)
            throw new Exception(room.GetPath() + ": Does not have a Volume script attached");

        //Is the voxelholder available?
        if (!volume.gameObjectContainer)
            throw new Exception(room.GetPath() + "'s Volume does not have a Game Object Container assigned");

        //Are the voxels empty
        if (volume.voxels.Count == 0)
            throw new Exception(room.GetPath() + ": has no voxels assigned");

        //Are there unassigned voxels
        foreach (Transform tr in volume.gameObjectContainer.transform)
        {
            if (!volume.voxels.Contains(tr.gameObject))
                throw new Exception(tr.GetPath() + ": is not assigned in volume: '" + volume.GetPath() + "'");
            if (!Regex.IsMatch(tr.name, " - \\((\\d+), (\\d+), (\\d+)\\)$"))
                Debug.LogWarning("There are gameobjects inside of gameObjectContainer which do not appear to be a voxel");
        }

        //does room contain doors, and are there unassigned doors?
        if (room.doors.Count == 0)
            throw new Exception(room.GetPath() + ": Does not have any doors");
        foreach (GeneratorDoor door in room.GetComponentsInChildren<GeneratorDoor>())
        {
            if(!room.doors.Contains(door))
                throw new Exception(room.GetPath() + ": contains the door: '" + door.GetPath() + "' But it is not assigned in the list");
            if(!door.voxelOwner)
                throw new Exception(door.GetPath() + ": does not have voxelOwner assigned");
            if(!Regex.IsMatch(door.voxelOwner.name, " - \\((\\d+), (\\d+), (\\d+)\\)$"))
                Debug.Log(door.GetPath() + "'s VoxelOwner does not appear to be a voxel");

            //TODO: Write test to check if the direction is not inside itself
            //TODO: Write test to warn if the VoxelOwner is not the closest voxel

            //TODO: Move this outside of the check function, I need to add more automatic solutions to this
            Vector3 doorPos = door.transform.position - room.transform.position;
            float halfGrid = Volume.VoxelScale * 0.5f;
            doorPos.x = halfGrid * Mathf.RoundToInt(doorPos.x / halfGrid);
            doorPos.y = halfGrid * Mathf.RoundToInt(doorPos.y / halfGrid);
            doorPos.z = halfGrid * Mathf.RoundToInt(doorPos.z / halfGrid);
            door.transform.localPosition = doorPos;

            Vector3 angles = door.transform.localEulerAngles;
            int yAngle = 90 * Mathf.RoundToInt(angles.y / 90f);
            door.transform.localRotation = Quaternion.Euler(0, yAngle, 0);
        }



        //TODO: Add checks for bounding box and voxel generation
    }

    private void CreateBaseFolderStructure()
    {
        //if (!AssetDatabase.IsValidFolder("Assets/Dungeons"))
        //    AssetDatabase.CreateFolder("Assets", "Dungeons");

        CreateFolderTo("Assets/Dungeons");
    }

    private void CreateFolderTo(string path, int from = 0)
    {
        char[] seperators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        int startIndex = @from == 0 ? 0 : path.IndexOfAny(seperators, @from) + 1;
        if (startIndex != -1)
        {
            int endIndex = path.IndexOfAny(seperators, startIndex);

            string foldername = endIndex != -1 ? path.Substring(startIndex, endIndex - startIndex) : path.Substring(startIndex);
            string precendingPath = path.Substring(0, startIndex);

            if (!AssetDatabase.IsValidFolder(precendingPath + foldername))
                AssetDatabase.CreateFolder(precendingPath.Remove(precendingPath.Length - 1), foldername);

            if (endIndex != -1)
            {
                CreateFolderTo(path, endIndex);
            }
        }
    }
}
