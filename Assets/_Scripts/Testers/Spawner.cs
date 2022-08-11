using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PlayerUnit;
    public GameObject EnemyUnit;
    public Transform SpawnPoint;

    public GameObject PlayerInstance;
    public GameObject EnemyInstance;

    public void SpawnPlayer()
    {
        var spawnPos = GameGrid.Instance.GeneratedGrid[4, 4].transform.position + (new Vector3(0f, 0.5f, 0f));
        EnemyInstance = Instantiate(EnemyUnit, spawnPos, Quaternion.identity);
        PlayerInstance = Instantiate(PlayerUnit, SpawnPoint.position, Quaternion.identity);
    }
}
