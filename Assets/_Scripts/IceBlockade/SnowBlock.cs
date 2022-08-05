using DG.Tweening;
using UnityEngine;

public class SnowBlock : MonoBehaviour
{
    public float SnowBuildupTime = 15f;
    public Collider BlockerCollider;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
        BlockerCollider = GetComponentInChildren<Collider>();
        BlockerCollider.GetComponent<Collider>().enabled = false;
        StartSnowBuildup();
    }

    private void StartSnowBuildup()
    {
        transform.DOScale(Vector3.one, SnowBuildupTime).OnComplete(() =>
        {
            BlockerCollider.GetComponent<Collider>().enabled = true;
        });
    }
}
