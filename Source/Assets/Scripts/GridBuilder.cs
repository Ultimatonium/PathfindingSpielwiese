using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridBuilder : MonoBehaviour
{
    [SerializeField]
    public int gridSize;
    [SerializeField]
    private bool hasDiagonalNeighbours;

    public Node[,] allNodes;

    private void Awake()
    {
        BuildNodes();
        AssignNeighbours();
    }

    private void BuildNodes()
    {
        allNodes = new Node[gridSize, gridSize];
        GameObject grid = new GameObject("Grid");
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Node newNode = new GameObject("Node " + x + "/" + y).AddComponent<Node>();
                newNode.neighbours = new List<Node>();
                newNode.gameObject.transform.parent = grid.transform;
                newNode.gameObject.transform.position = new Vector3(x, y);
                newNode.position = newNode.transform.position;
                newNode.passability = Mathf.PerlinNoise(x * 0.5f, y * 0.5f);
                //newNode.passability = 1;
                allNodes[x, y] = newNode;
            }
        }
    }

    private void AssignNeighbours()
    {
        for (int x = 0; x < gridSize; x++)
        {
            bool isLeftEdge = x == 0;
            bool isRightEdge = x == gridSize - 1;
            for (int y = 0; y < gridSize; y++)
            {
                bool isBottomEdge = y == 0;
                bool isTopEdge = y == gridSize - 1;
                if (!isTopEdge && !isLeftEdge && hasDiagonalNeighbours)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x - 1, y + 1]);
                }
                if (!isTopEdge)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x, y + 1]);
                }
                if (!isTopEdge && !isRightEdge && hasDiagonalNeighbours)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x + 1, y + 1]);
                }
                if (!isRightEdge)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x + 1, y]);
                }
                if (!isBottomEdge && !isRightEdge && hasDiagonalNeighbours)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x + 1, y - 1]);
                }
                if (!isBottomEdge)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x, y - 1]);
                }
                if (!isBottomEdge && !isLeftEdge && hasDiagonalNeighbours)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x - 1, y - 1]);
                }
                if (!isLeftEdge)
                {
                    allNodes[x, y].neighbours.Add(allNodes[x - 1, y]);
                }
            }
        }
    }
}
