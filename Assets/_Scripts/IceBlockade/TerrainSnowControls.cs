using DG.Tweening;
using UnityEngine;

public class TerrainSnowControls : MonoBehaviour
{
    public float SnowpileRiseTime = 10f;
    public float RiseLevel = -0.111f;

    private void Start()
    {
        BeginSnowBuildup();
    }

    private void BeginSnowBuildup()
    {
        transform.DOLocalMoveY(RiseLevel, SnowpileRiseTime);
    }
}
