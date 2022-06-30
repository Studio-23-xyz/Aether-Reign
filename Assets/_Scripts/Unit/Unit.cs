using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Unit : SerializedMonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    public LayerMask TileLayer;
    public bool IsMoving;
    public bool IsAimingAbility;
    public bool IsCastingAbility;
    public Vector3 TileOffset;

    [SerializeField] private int _actionPoints;
    public List<ISpell> SpellGrimoire = new List<ISpell>();


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        GetActableTiles(_actionPoints, isSpell: false);
    }

    private void GetActableTiles(int tileRange, bool isSpell)
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
                            cell.SetTileVisibility(true);
                            cell.SetTileVisualColor(isSpell);
                            cell.IsWalkable = true;
                        }
                    }
                }
            }
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsAimingAbility)
            {
                IsAimingAbility = false;
                GameGrid.Instance.DisableWalkable();
                GetActableTiles(_actionPoints, isSpell: false);
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 hitPos = GetMouseWorldPosition();
            if (hitPos == Vector3.zero)
                return;
            if (!IsMoving || !IsCastingAbility || !IsAimingAbility)
            {
                NavMeshPath targetPath = new NavMeshPath();
                _agent.CalculatePath(hitPos + TileOffset, targetPath);
                _agent.SetPath(targetPath);
                WaitForMoveFinish();
                //TileMovement(GameGrid.Instance.GetPath(transform.position, hitPos));
            }

            if (IsAimingAbility)
            {
                IsAimingAbility = false;
                SpellGrimoire[0].CastSpell(transform.position, hitPos + TileOffset);
                GetActableTiles(_actionPoints, false);
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsMoving || IsCastingAbility || IsAimingAbility) return;

            IsAimingAbility = true;
            GameGrid.Instance.DisableWalkable();
            if (AttemptToCastSpell())
            {
                GetActableTiles(SpellGrimoire[0].SpellRange, isSpell: true);
            }
        }
    }

    private bool AttemptToCastSpell()
    {
        if (IsMoving || IsCastingAbility)
            return false;
        Debug.Log($"Attempt successful");
        return true;
    }

    private async void WaitForMoveFinish()
    {
        IsMoving = true;
        _animator.SetBool("IsMoving", true);
        GameGrid.Instance.DisableWalkable();
        while (_agent.hasPath)
        {
            await UniTask.Yield();
            await UniTask.NextFrame();
        }

        IsMoving = false;
        _animator.SetBool("IsMoving", false);
        GetActableTiles(_actionPoints, isSpell: false);
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
