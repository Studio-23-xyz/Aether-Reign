using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PlayerUnit;
    public GameObject EnemyUnit;

    public GameObject PlayerInstance;
    public GameObject EnemyInstance;

    public void SpawnPlayer()
    {
        var randomTile1 = GameGrid.Instance.GetRandomTile();
        var randomTile2 = GameGrid.Instance.GetRandomTile();

        //var spawnPos = GameGrid.Instance.GeneratedGrid[4, 4].transform.position + (new Vector3(0f, 0.5f, 0f));
        EnemyInstance = Instantiate(EnemyUnit, randomTile2.transform.position + new Vector3(0f, 0.5f, 0.5f), Quaternion.identity);
        //PlayerInstance = Instantiate(PlayerUnit, randomTile1.transform.position, Quaternion.identity);
    }


}
