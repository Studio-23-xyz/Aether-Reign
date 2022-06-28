using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    public LayerMask TileLayer;
    public bool IsMoving;
    public Vector3 TileOffset;

    [SerializeField] private int _actionPoints;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        GetWalkeableTiles();
    }

    private void GetWalkeableTiles()
    {
        GameGrid.Instance.GetXZ(transform.position, out int unitX, out int unitZ);
        int lowX = unitX - _actionPoints;
        int lowZ = unitZ - _actionPoints;
        int highX = unitX + _actionPoints;
        int highZ = unitZ + _actionPoints;

        for (int x = lowX; x <= highX; x++)
        {
            for (int y = lowZ; y <= highZ; y++)
            {
                if (!GameGrid.Instance.IsWithinGrid(x,y))
                    continue;

                if (GameGrid.Instance.GetPath(new Vector3(unitX, 0f, unitZ), new Vector3(x, 0f, y)).Count <=
                    _actionPoints)
                {
                    var cell = GameGrid.Instance.GeneratedGrid[x, y].GetComponent<GridCell>();

                    if (!cell.IsOccupied)
                    {
                        NavMeshPath path = new NavMeshPath();
                        if (_agent.CalculatePath(cell.transform.position + TileOffset, path))
                        {
                            cell.SetMoveTileVisibility(true);
                            cell.IsWalkable = true;
                        }
                    }
                }
            }
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMoving) return;

            Vector3 hitPos = GetMouseWorldPosition();
            if (hitPos == Vector3.zero)
                return;
            NavMeshPath targetPath = new NavMeshPath();
            _agent.CalculatePath(hitPos + TileOffset, targetPath);
            _agent.SetPath(targetPath);
            WaitForMoveFinish();
            //TileMovement(GameGrid.Instance.GetPath(transform.position, hitPos));
        }
    }

    private async void WaitForMoveFinish()
    {
        IsMoving = true;
        _animator.SetBool("IsMoving", true);
        GameGrid.Instance.DisableWalkable();
        while (_agent.hasPath)
        {
            Debug.Log($"{_agent.hasPath}");
            
            await UniTask.Yield();
            await UniTask.NextFrame();
        }

        IsMoving = false;
        _animator.SetBool("IsMoving", false);
        GetWalkeableTiles();
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
