using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private int _xPos, _yPos;
    public bool IsWalkable;
    public bool IsOccupied;
    [ShowInInspector] public Color MoveColor;
    [ShowInInspector] public Color SpellTileColor;

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
