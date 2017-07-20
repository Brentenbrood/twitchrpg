using System.Collections;
using System.Collections.Generic;
using EL.Dungeon;
using UnityEngine;

public class BossRoom : Room
{
    public GameObject BossPrefab;
    public Transform SpawnPosition;

    public GameObject SpawnBoss()
    {
        return Instantiate(BossPrefab, SpawnPosition.position, Quaternion.identity);
    }
}
