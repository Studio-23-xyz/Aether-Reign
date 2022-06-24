using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private int _xPos, _yPos;
    public bool IsWalkable;
    public bool IsOccupied;
    public List<GridCell> NeighborCells = new List<GridCell>();

    private void Start()
    {
        var originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(originalScale + new Vector3(0.2f, 0.2f, 0.2f), 0.5f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.2f);
        });
    }

    public void SetParameters(int x, int y, bool walkable, bool occupied)
    {
        _xPos = x;
        _yPos = y;
        IsWalkable = walkable;
        IsOccupied = occupied;
    }
}
