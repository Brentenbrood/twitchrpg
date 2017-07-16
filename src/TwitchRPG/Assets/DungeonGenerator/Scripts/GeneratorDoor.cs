using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EL.Dungeon {
    public class GeneratorDoor : MonoBehaviour {
        public bool open = true;
        public GameObject voxelOwner;
        public GeneratorDoor sharedDoor;

        public Door door;

        private Volume volume;
        public Volume Volume
        {
            get
            {
                if (!volume)
                    volume = GetComponentInParent<Volume>();
                return volume;
            }
        }

        public Vector3 GetDirectionPoint()
        {
            float rot = TransformUtils.NormalizeAngle(Mathf.RoundToInt(transform.rotation.eulerAngles.y));
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
            else
            {
                Debug.LogWarning("Y rotation is not on a 90 degree scale");
            }

            return voxelOwner.transform.position + (direction * Volume.VoxelScale);
        }

        [ContextMenu("Assign Nearest Voxel")]
        public void AssignNearestVoxel()
        {
            GameObject closest = null;
            float closestDistance = int.MaxValue;
            foreach (GameObject voxel in Volume.voxels)
            {
                float dist = Vector3.Distance(transform.position, voxel.transform.position);
                if (dist < closestDistance)
                {
                    closest = voxel;
                    closestDistance = dist;
                }
            }

            if(!closest)
                Debug.Log("No voxel found, is the voxel list in the Volume empty?");

            voxelOwner = closest;
        }
    }

}
