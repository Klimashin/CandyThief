using UnityEngine;

public class PathfindingNode
{
    public bool Walkable;
    public Vector3Int WorldPosition;
    public int GridX;
    public int GridY;

    public int GCost;
    public int HCost;
    public PathfindingNode Parent;
	
    public PathfindingNode(bool walkable, Vector3Int worldPos, int gridX, int gridY) 
    {
        Walkable = walkable;
        WorldPosition = worldPos;
        GridX = gridX;
        GridY = gridY;
    }

    public int FCost => GCost + HCost;
}