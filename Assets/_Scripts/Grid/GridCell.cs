using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private int _xPos, _yPos;
    public bool IsWalkable;
    public bool IsOccupied;
    public List<GridCell> NeighborCells = new List<GridCell>();

    public GameObject MoveVisibility;

    private void Start()
    {
        MoveVisibility.SetActive(false);
        var originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(originalScale + new Vector3(0.2f, 0.2f, 0.2f), 0.5f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.2f);
        });
    }

    public void SetMoveTileVisibility(bool state) => MoveVisibility.SetActive(state);

    public void SetParameters(int x, int y, bool walkable, bool occupied)
    {
        _xPos = x;
        _yPos = y;
        IsWalkable = walkable;
        IsOccupied = occupied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit") || other.CompareTag("Obstacle"))
        {
            IsWalkable = false;
            IsOccupied = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IsWalkable = true;
        IsOccupied = true;
    }
}
