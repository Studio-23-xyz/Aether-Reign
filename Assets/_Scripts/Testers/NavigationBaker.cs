using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavigationBaker : MonoBehaviour
{
    public List<NavMeshSurface> GridCells;
    public bool BakeNavMesh;

    // Use this for initialization
    void Update()
    {
        if (BakeNavMesh)
        {
            foreach (var navMeshSurface in GridCells)
            {
                navMeshSurface.BuildNavMesh();
            }
        }
    }

}