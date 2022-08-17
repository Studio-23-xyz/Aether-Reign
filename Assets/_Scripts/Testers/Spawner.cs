using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PlayerUnit;
    public GameObject EnemyUnit;

    public GameObject PlayerInstance;
    public GameObject EnemyInstance;

    public async UniTask SpawnUnits()
    {
        GameObject tile_1 = null, tile_2 = null;

        while (tile_1 == tile_2)
        {
            tile_1 = GameGrid.Instance.GetRandomTile();
            tile_2 = GameGrid.Instance.GetRandomTile();
        }

        PlayerInstance = Instantiate(PlayerUnit, tile_1.transform.position, Quaternion.identity);

        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));

        EnemyInstance = Instantiate(EnemyUnit, tile_2.transform.position + new Vector3(0f, 0.5f, 0.5f), Quaternion.identity);
    }
}
