using System.Diagnostics;
using CodeMonkey.Utils;
using UnityEngine;

public class Grid
{
    private int _width;
    private int _height;
    private float _cellSize;

    private int[,] _gridArray;

    public Grid(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridArray = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                UtilsClass.CreateWorldText(_gridArray[i, j].ToString(), null, GetWorldPosition(i, j), 20, Color.red, TextAnchor.MiddleCenter);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * _cellSize;
    }
}
