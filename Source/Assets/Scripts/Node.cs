using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IComparable<Node>
{
    public float passability;
    public float cost;
    public Vector3 position;
    public List<Node> neighbours;

    private void OnDrawGizmos()
    {
        Color lineColor = Color.blue;
        lineColor.a *= 0.1f;
        Gizmos.color = lineColor;
        foreach (Node neighbour in neighbours)
        {
            Gizmos.DrawLine(position, neighbour.position);
        }
        //return;
        Gizmos.color = new Color(passability,passability,passability);
        Gizmos.DrawSphere(position, passability / 2);
    }

    public int CompareTo(Node other)
    {
        if (other.cost > cost) return -1;
        if (other.cost < cost) return 1;
        return 0;
    }
}
