using UnityEngine;

public class GridTestInit : MonoBehaviour
{
    public int Width;
    public int Height;
    public int CellSize;
    private void Start()
    {
        Grid newGrid = new Grid(Width, Height, 5f);
    }
}
