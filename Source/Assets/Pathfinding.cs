using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Pathfinding : MonoBehaviour
{
    [SerializeField]
    private PathfindingType pathfindingType;
    [SerializeField]
    private Vector2 startNodePosition;
    [SerializeField]
    private Vector2 endNodePosition;

    private List<Node> path = new List<Node>();
    private Dictionary<Node, Node> directionMap = new Dictionary<Node, Node>();
    private Node startNode;
    private Node endNode;

    private PathfindingType prevPathfindingType;

    private void Start()
    {
        prevPathfindingType = pathfindingType;
        CalcPath();
    }

    private void Update()
    {
        if (prevPathfindingType != pathfindingType)
        {
            path.Clear();
            directionMap.Clear();
            prevPathfindingType = pathfindingType;
            CalcPath();
        }
    }

    private void CalcPath()
    {
        if (startNodePosition != Vector2.zero && endNodePosition != Vector2.zero)
        {
            startNode = GetNode((int)startNodePosition.x, (int)startNodePosition.y);
            endNode = GetNode((int)endNodePosition.x, (int)endNodePosition.y);
        }
        else
        {
            startNode = GetRandomNode();
            endNode = GetRandomNode();
        }
        DateTime startTimeCSharp = DateTime.Now;
        float startTimeUnity = Time.realtimeSinceStartup;
        switch (pathfindingType)
        {
            case PathfindingType.BreathFirst:
                BreathFirst(startNode, endNode, ref path, ref directionMap);
                break;
            case PathfindingType.DepthFirst:
                DepthFirst(startNode, endNode, ref path, ref directionMap);
                break;
            case PathfindingType.Dijkstra:
                Dijkstra(startNode, endNode, ref path, ref directionMap);
                break;
            case PathfindingType.AStar:
                AStar(startNode, endNode, ref path, ref directionMap);
                break;
            default:
                Debug.LogError(Enum.GetName(typeof(PathfindingType), pathfindingType) + " not handled");
                break;
        }
        Debug.Log($"Path calculation with {Enum.GetName(typeof(PathfindingType), pathfindingType)} C#:{(DateTime.Now - startTimeCSharp)}|Unity:{TimeSpan.FromSeconds(Time.realtimeSinceStartup - startTimeUnity)}");
    }

    private Node GetRandomNode()
    {
        GridBuilder gridBuilder = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridBuilder>();
        return GetNode(Random.Range(0, gridBuilder.gridSize), Random.Range(0, gridBuilder.gridSize));
    }

    private Node GetNode(int x, int y)
    {
        GridBuilder gridBuilder = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridBuilder>();
        return gridBuilder.allNodes[x, y];
    }

    protected static List<Node> TracePathBackwards(Dictionary<Node, Node> directionMap, Node to)
    {
        List<Node> path = new List<Node>();
        Node back = to;
        while (back != null)
        {
            path.Add(back);
            back = directionMap[back];
        }
        path.Reverse();
        return path;
    }

    enum PathfindingType
    {
        BreathFirst,
        DepthFirst,
        Dijkstra,
        AStar
    }

    public static void BreathFirst(Node from, Node to, ref List<Node> path, ref Dictionary<Node, Node> directionMap)
    {
        bool targetFound = false;
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(from);
        directionMap.Add(from, null);
        while (q.Count > 0 && !targetFound)
        {
            Node currentNode = q.Dequeue();
            foreach (Node neighbour in currentNode.neighbours)
            {
                if (directionMap.ContainsKey(neighbour)) continue;
                directionMap.Add(neighbour, currentNode);
                q.Enqueue(neighbour);
                if (neighbour == to)
                {
                    targetFound = true;
                    break;
                }
            }
        }
        path = TracePathBackwards(directionMap, to);
    }

    public static void DepthFirst(Node from, Node to, ref List<Node> path, ref Dictionary<Node, Node> directionMap)
    {
        bool targetFound = false;
        Stack<Node> q = new Stack<Node>();
        q.Push(from);
        directionMap.Add(from, null);
        while (q.Count > 0 && !targetFound)
        {
            Node currentNode = q.Pop();
            foreach (Node neighbour in currentNode.neighbours)
            {
                if (directionMap.ContainsKey(neighbour)) continue;
                directionMap.Add(neighbour, currentNode);
                q.Push(neighbour);
                if (neighbour == to)
                {
                    targetFound = true;
                    break;
                }
            }
        }
        path = TracePathBackwards(directionMap, to);
    }

    public static void Dijkstra(Node from, Node to, ref List<Node> path, ref Dictionary<Node, Node> directionMap)
    {
        bool targetFound = false;
        PriorityQueue<Node> q = new PriorityQueue<Node>();
        //Dictionary<Node, float> costMap = new Dictionary<Node, float>();
        //costMap.Add(from, 0);
        q.Enqueue(from);
        directionMap.Add(from, null);
        while (q.Count > 0 && !targetFound)
        {
            Node currentNode = q.Dequeue();
            foreach (Node neighbour in currentNode.neighbours)
            {
                if (directionMap.ContainsKey(neighbour)) continue;
                //float costNeighbour = (costMap[currentNode] + 1) * neighbour.passability;
                float costNeighbour = (currentNode.cost + 1) * neighbour.passability;
                //costMap.Add(neighbour, costNeighbour);
                neighbour.cost = costNeighbour;
                directionMap.Add(neighbour, currentNode);
                q.Enqueue(neighbour);
                if (neighbour == to)
                {
                    targetFound = true;
                    break;
                }
            }
        }
        path = TracePathBackwards(directionMap, to);
    }

    public static void AStar(Node from, Node to, ref List<Node> path, ref Dictionary<Node, Node> directionMap)
    {
        bool targetFound = false;
        PriorityQueue<Node> q = new PriorityQueue<Node>();
        q.Enqueue(from);
        directionMap.Add(from, null);
        while (q.Count > 0 && !targetFound)
        {
            Node currentNode = q.Dequeue();
            foreach (Node neighbour in currentNode.neighbours)
            {
                if (directionMap.ContainsKey(neighbour)) continue;
                float costNeighbour = (currentNode.cost + 1) * neighbour.passability;
                costNeighbour += Vector3.Distance(neighbour.position, to.position);
                neighbour.cost = costNeighbour;
                directionMap.Add(neighbour, currentNode);
                q.Enqueue(neighbour);
                if (neighbour == to)
                {
                    targetFound = true;
                    break;
                }
            }
        }
        path = TracePathBackwards(directionMap, to);
    }

    private void OnDrawGizmos()
    {
        foreach (KeyValuePair<Node, Node> direction in directionMap)
        {
            if (direction.Key == null || direction.Value == null) continue;
            Handles.ArrowHandleCap(0, direction.Key.position, Quaternion.LookRotation(direction.Value.position - direction.Key.position), 1, EventType.Repaint);
        }
        if (startNode == null) return;
        if (endNode == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(startNode.position, Vector3.one);
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(endNode.position, Vector3.one);
        foreach (Node node in path)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(node.position, Vector3.one / 2);
        }
    }
}
