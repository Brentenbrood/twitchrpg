using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EL.Dungeon
{
    //by maiko steeman
    [CustomEditor(typeof(GeneratorDoor))]
    [CanEditMultipleObjects]
    public class GeneratorDoorEditor : Editor
    {
        private bool DrawDirection = false;

        private GeneratorDoor generatorDoor;
        public GeneratorDoor GeneratorDoor
        {
            get
            {
                if (!generatorDoor)
                    generatorDoor = (GeneratorDoor) target;
                return generatorDoor;
            }
        }

        void OnEnable()
        {
            Selection.selectionChanged += SelectionChanged;
        }

        private void SelectionChanged()
        {
            if(GeneratorDoor)
                DrawDirection = Selection.gameObjects.Contains(GeneratorDoor.gameObject);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Separator();

            DrawDirection = EditorGUILayout.Toggle("Draw Direction", DrawDirection);
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        static void DrawGizmoForTarget(GeneratorDoor door, GizmoType type)
        {
            /*if (!door.DrawDirection)
                return;*/

            if (!door.Volume || !door.voxelOwner)
                return;

            Gizmos.color = Color.red;

            Vector3 doorDirection = door.GetDirectionPoint();
            
            Gizmos.DrawSphere(doorDirection, 1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(door.voxelOwner.transform.position, door.transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(door.transform.position, doorDirection);

            if (door.sharedDoor)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(doorDirection, door.sharedDoor.GetDirectionPoint());
            }
        }
    } 
}
