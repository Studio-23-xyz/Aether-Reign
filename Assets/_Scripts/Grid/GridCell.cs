using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private int _xPos, _yPos;
    public bool IsWalkable;
    public bool IsOccupied;
    public Color MoveColor;
    public Color SpellTileColor;

    public CellState CurrentCellState;

    public GameObject MoveVisibility;
    public GameObject SpellAoEVisual;
    public bool SpellAoEIsActive;

    private void Start()
    {
        MoveVisibility.SetActive(false);
        SpellAoEVisual.SetActive(false);
        var originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(originalScale + new Vector3(0.2f, 0.2f, 0.2f), 0.5f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.2f);
        });
    }

    public void SetTileVisibility(bool state)
    {
        MoveVisibility.SetActive(state);
    }

    public void SetTileVisualColor(bool isSpell)
    {
        var tileColor = isSpell ? SpellTileColor : MoveColor;
        MoveVisibility.GetComponent<SpriteRenderer>().color = tileColor;
    }

    public void SetParameters(int x, int y, bool walkable, bool occupied)
    {
        _xPos = x;
        _yPos = y;
        IsWalkable = walkable;
        IsOccupied = occupied;
        CurrentCellState = CellState.Normal;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter called");
        if (other.CompareTag(StringResources.PlayerTag) || other.CompareTag(StringResources.EnemyTag) || other.CompareTag("Obstacle"))
        {
            IsWalkable = false;
            IsOccupied = true;
        }

        //Debug.Log($"Tile {gameObject.name}, IsWalkable: {IsWalkable} & IsOccupied: {IsOccupied}");
    }

    private void OnTriggerExit(Collider other)
    {
        IsWalkable = true;
        IsOccupied = false;

        //Debug.Log($"Tile {gameObject.name}, IsWalkable: {IsWalkable} & IsOccupied: {IsOccupied}");
    }
}
