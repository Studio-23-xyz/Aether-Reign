using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public static GameGrid Instance;
    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private float _gridSpacing;

    [SerializeField] private GameObject _gridCellPrefab;

    private GameObject _gridParent;

    private GameObject[,] _gameGrid;

    private void Start()
    {
        Instance = this;
    }

    public List<Vector3> GetPath(Vector3 fromPosition, Vector3 ToPosition)
    {
        GetXZ(fromPosition, out int fromX, out int fromZ);
        GetXZ(ToPosition, out int toX, out int toZ);

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

    private void GetXZ(Vector3 worldPosition, out int x, out int z)
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

        _gameGrid = new GameObject[_height, _width];

        CreateGrid();
    }

    private void CreateGrid()
    {
        if (_gridCellPrefab == null)
            Debug.LogError($"No prefab assigned");

        _gridParent = Instantiate(new GameObject("GridParent"), Vector3.zero, Quaternion.identity);

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _gameGrid[x, y] = Instantiate(_gridCellPrefab, new Vector3(x, 0f, y) * _gridSpacing, Quaternion.identity, _gridParent.transform);
                _gameGrid[x, y].name = $"Cell {x},{y}";
                _gameGrid[x, y].GetComponent<GridCell>().SetParameters(x, y, walkable: true, occupied: false);
            }
        }

        //InitiateCells();
    }

    private void InitiateCells()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                AssignWalkableNeighbors(x, y);
            }
        }
    }

    private void AssignWalkableNeighbors(int xPos, int yPos)
    {
        for (int i = yPos - 1; i <= yPos + 1; i++)
        {
            for (int j = xPos - 1; j <= xPos + 1; j++)
            {
                //Debug.Log($"For Cell, [{xPos} , {yPos}]. Currently iterating XPos: {j}, YPos: {i}");
                if ((i == yPos && j == xPos))
                    continue;

                if (WithinGrid(i, j))
                {
                    GridCell currentCell;
                    currentCell = _gameGrid[i, j].GetComponent<GridCell>();

                    if (currentCell.IsWalkable)
                    {
                        _gameGrid[xPos, yPos].GetComponent<GridCell>().NeighborCells.Add(currentCell);
                        Debug.Log($"For Cell, [{xPos} , {yPos}], added neighbor {i}, {j}");
                    }
                }
            }
        }
    }

    private bool WithinGrid(int col, int row)
    {
        if (col < 0 || row < 0)
            return false;
        if (row >= _height || col >= _width)
            return false;
        return true;
    }

    [ContextMenu("Regenerate")]
    public void RegenrateGrid()
    {
        foreach (Transform child in _gridParent.transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(_gridParent);

        InitiateGrid(_height, _width, _gridSpacing);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 200, 40), "Regenerate Grid"))
        {
            RegenrateGrid();
        }
    }
}
