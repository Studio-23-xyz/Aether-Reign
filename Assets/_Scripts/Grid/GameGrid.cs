using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameGrid : MonoBehaviour
{
    public static GameGrid Instance;
    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private float _gridSpacing;

    [SerializeField] private GameObject _gridCellPrefab;

    private GameObject _gridParent;

    public GameObject[,] GeneratedGrid;

    private void Start()
    {
        Instance = this;
    }

    public List<Vector3> GetPath(Vector3 fromPosition, Vector3 toPosition)
    {
        GetXZ(fromPosition, out int fromX, out int fromZ);
        GetXZ(toPosition, out int toX, out int toZ);

        List<Vector3> path = new List<Vector3>();

        while (toX > fromX)
        {
            fromX++;
            path.Add(GetWorldPosition(fromX, fromZ));
        }

        while (toX < fromX)
        {
            fromX--;
            path.Add(GetWorldPosition(fromX, fromZ));
        }

        while (toZ > fromZ)
        {
            fromZ++;
            path.Add(GetWorldPosition(fromX, fromZ));
        }

        while (toZ < fromZ)
        {
            fromZ--;
            path.Add(GetWorldPosition(fromX, fromZ));
        }

        return path;
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0f, y) * _gridSpacing;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPosition.x / _gridSpacing);
        z = Mathf.FloorToInt(worldPosition.z / _gridSpacing);
    }

    public void DebugButtonClickGridGeneration()
    {
        InitiateGrid(_height, _width, _gridSpacing);
    }

    public void InitiateGrid(int height, int width, float spacing = 0f)
    {
        _height = height;
        _width = width;
        _gridSpacing = spacing;

        GeneratedGrid = new GameObject[_height, _width];

        CreateGrid();
    }

    private async void CreateGrid()
    {
        if (_gridCellPrefab == null)
            Debug.LogError($"No prefab assigned");

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                GeneratedGrid[x, y] = Instantiate(_gridCellPrefab, new Vector3(x, 0f, y) * _gridSpacing, Quaternion.identity, transform);
                GeneratedGrid[x, y].name = $"Cell {x},{y}";
                GeneratedGrid[x, y].GetComponent<GridCell>().SetParameters(x, y, walkable: false, occupied: false);
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            }
        }

        GeneratedGrid[0, 0].GetComponent<NavMeshSurface>().BuildNavMesh();

        //InitiateCells();
    }

    //private void InitiateCells()
    //{
    //    for (int y = 0; y < _height; y++)
    //    {
    //        for (int x = 0; x < _width; x++)
    //        {
    //            AssignWalkableNeighbors(x, y);
    //        }
    //    }
    //}

    //private void AssignWalkableNeighbors(int xPos, int yPos)
    //{
    //    for (int i = yPos - 1; i <= yPos + 1; i++)
    //    {
    //        for (int j = xPos - 1; j <= xPos + 1; j++)
    //        {
    //            //Debug.Log($"For Cell, [{xPos} , {yPos}]. Currently iterating XPos: {j}, YPos: {i}");
    //            if ((i == yPos && j == xPos))
    //                continue;

    //            if (IsWithinGrid(i, j))
    //            {
    //                GridCell currentCell;
    //                currentCell = GeneratedGrid[i, j].GetComponent<GridCell>();

    //                if (currentCell.IsWalkable)
    //                {
    //                    GeneratedGrid[xPos, yPos].GetComponent<GridCell>().NeighborCells.Add(currentCell);
    //                    Debug.Log($"For Cell, [{xPos} , {yPos}], added neighbor {i}, {j}");
    //                }
    //            }
    //        }
    //    }
    //}

    public bool IsWithinGrid(int col, int row)
    {
        if (col < 0 || row < 0)
            return false;
        if (row >= _height || col >= _width)
            return false;
        return true;
    }

    public void DisableWalkable()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var cell = GeneratedGrid[x, y].GetComponent<GridCell>(); 
                cell.SetTileVisibility(false);
                cell.IsWalkable = false;
            }
        }
    }

    public void DisableSpellAoEVisuals()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var cell = GeneratedGrid[x, y].GetComponent<GridCell>();
                cell.SpellAoEVisual.SetActive(false);
                cell.SpellAoEIsActive = false;
            }
        }
    }

    public List<GameObject> GetActableTiles(int tileRange, bool isSpell, Transform origin, bool shouldHighlight = true)
    {
        List<GameObject> actableTiles = new List<GameObject>();
        GetXZ(origin.position, out var unitX, out var unitZ);
        var lowX = unitX - tileRange;
        var lowZ = unitZ - tileRange;
        var highX = unitX + tileRange;
        var highZ = unitZ + tileRange;

        for (var x = lowX; x <= highX; x++)
        for (var y = lowZ; y <= highZ; y++)
        {
            if (!IsWithinGrid(x, y))
                continue;

            if (GetPath(new Vector3(unitX, 0f, unitZ), new Vector3(x, 0f, y)).Count <=
                tileRange)
            {
                actableTiles.Add(GeneratedGrid[x, y]);
                var cell = GeneratedGrid[x, y].GetComponent<GridCell>();

                if (!cell.IsOccupied)
                {
                    cell.SetTileVisibility(shouldHighlight);
                    cell.SetTileVisualColor(isSpell);
                    cell.IsWalkable = true;
                }
            }
        }

        return actableTiles;
    }

    public List<GameObject> GetSpellAoETiles(int tileRange, Transform origin)
    {
        List<GameObject> actableTiles = new List<GameObject>();
        GetXZ(origin.position, out var unitX, out var unitZ);
        var lowX = unitX - tileRange;
        var lowZ = unitZ - tileRange;
        var highX = unitX + tileRange;
        var highZ = unitZ + tileRange;

        for (var x = lowX; x <= highX; x++)
        for (var y = lowZ; y <= highZ; y++)
        {
            if (!IsWithinGrid(x, y))
                continue;

            if (GetPath(new Vector3(unitX, 0f, unitZ), new Vector3(x, 0f, y)).Count <=
                tileRange)
            {
                actableTiles.Add(GeneratedGrid[x, y]);
            }
        }

        return actableTiles;
    }

    public GameObject GetRandomTile()
    {
        var randomX = Random.Range(0, GeneratedGrid.GetUpperBound(0));
        var randomY = Random.Range(0, GeneratedGrid.GetUpperBound(1));
        return GeneratedGrid[randomX, randomY];
    }

    [ContextMenu("Regenerate")]
    public void RegenrateGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //Destroy(_gridParent);

        InitiateGrid(_height, _width, _gridSpacing);
    }
}
