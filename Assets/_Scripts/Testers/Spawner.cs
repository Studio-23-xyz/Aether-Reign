using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PlayerUnit;
    public Transform SpawnPoint;

    public GameObject PlayerInstance;

    public void SpawnPlayer()
    {
        PlayerInstance = Instantiate(PlayerUnit, SpawnPoint.position, Quaternion.identity);
    }
}
