using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Animator _animator;
    public LayerMask TileLayer;
    public bool IsMoving;
    public Vector3 TileOffset;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMoving) return;

            Vector3 hitPos = GetMouseWorldPosition();
            if (hitPos == Vector3.zero)
                return;

            TileMovement(GameGrid.Instance.GetPath(transform.position, hitPos));
        }
    }

    private async void TileMovement(List<Vector3> path)
    {
        IsMoving = true;
        _animator.SetBool("IsMoving", true);
        foreach (var tilePos in path)
        {
            var direction = (tilePos + TileOffset) - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);

            while (transform.position != tilePos + TileOffset)
            {
                transform.position = Vector3.MoveTowards(transform.position, tilePos + TileOffset, 0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
            }
            await UniTask.Yield();
            await UniTask.NextFrame();
        }
        IsMoving = false;
        _animator.SetBool("IsMoving", false);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out var hitInfo, TileLayer))
        {
            if (hitInfo.collider.GetComponent<GridCell>())
            {
                if (hitInfo.collider.GetComponent<GridCell>().IsWalkable)
                    return hitInfo.transform.position;
                else
                {
                    Debug.LogWarning($"Tile not walkable");
                    return Vector3.zero;
                }
            }
        }

        Debug.LogWarning($"No tile hit");
        return Vector3.zero;
    }
}
