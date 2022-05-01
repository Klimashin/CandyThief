using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _obstaclesTilemap;
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    public Grid GetGrid() => _grid;
    public Tilemap GetObstaclesTilemap() => _obstaclesTilemap;
    public Tilemap GetFloorTilemap() => _floorTilemap;

    public Vector3 CellToWorld(Vector3Int gridPosition)
    {
        return _grid.GetCellCenterWorld(gridPosition);
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return _grid.WorldToCell(worldPos);
    }

    public bool IsCellFree(Vector3Int gridPosition)
    {
        return _obstaclesTilemap.GetTile(gridPosition) == null;
    }
    
    public static Vector3Int GetVectorInDirection(MoveDirection dir, int length)
    {
        switch (dir)
        {
            case MoveDirection.Down:
                return new Vector3Int(0, -length, 0);
            
            case MoveDirection.Up:
                return new Vector3Int(0, length, 0);
            
            case MoveDirection.Left:
                return new Vector3Int(-length, 0, 0);
            
            case MoveDirection.Right:
                return new Vector3Int(length, 0, 0);
            
            default:
                throw new Exception("unknown direction");
        }
    }
    
    private PathfindingNode[,] _pfGrid;
    private Vector3Int _pfGridBottomLeft;
    
    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        _pfGrid = new PathfindingNode[mapWidth, mapHeight];
        _pfGridBottomLeft  = new Vector3Int(-mapWidth/2, -mapHeight/2, 0);

		for (int x = 0; x < mapWidth; x ++) 
        {
			for (int y = 0; y < mapHeight; y ++) 
            {
				Vector3Int worldGridCell = new Vector3Int(x, y, 0) + _pfGridBottomLeft;
				bool walkable = _obstaclesTilemap.GetTile(worldGridCell) == null;
				_pfGrid[x,y] = new PathfindingNode(walkable, worldGridCell, x, y);
			}
		}
    }

    private PathfindingNode PfNodeFromWorldCell(Vector3Int worldCell)
    {
        var nodeIndex = worldCell - _pfGridBottomLeft;
        return _pfGrid[nodeIndex.x, nodeIndex.y];
    }
    
    private List<PathfindingNode> GetNeighbours(PathfindingNode pathfindingNode) 
    {
    	List<PathfindingNode> neighbours = new List<PathfindingNode>();
        if (pathfindingNode.GridX - 1 >= 0)
            neighbours.Add(_pfGrid[pathfindingNode.GridX - 1, pathfindingNode.GridY]);
        
        if (pathfindingNode.GridX + 1 < mapWidth)
            neighbours.Add(_pfGrid[pathfindingNode.GridX + 1, pathfindingNode.GridY]);
        
        if (pathfindingNode.GridY - 1 >= 0)
            neighbours.Add(_pfGrid[pathfindingNode.GridX, pathfindingNode.GridY - 1]);
        
        if (pathfindingNode.GridY + 1 < mapHeight)
            neighbours.Add(_pfGrid[pathfindingNode.GridX, pathfindingNode.GridY + 1]);

        return neighbours;
    }

    public List<Vector3Int> FindPath(Vector3Int startPos, Vector3Int targetPos) 
    {
        PathfindingNode startPathfindingNode = PfNodeFromWorldCell(startPos);
        PathfindingNode targetPathfindingNode = PfNodeFromWorldCell(targetPos);

        List<PathfindingNode> openSet = new List<PathfindingNode>();
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
        openSet.Add(startPathfindingNode);

        while (openSet.Count > 0) 
        {
            PathfindingNode pathfindingNode = openSet[0];
            for (int i = 1; i < openSet.Count; i ++)
            {
                if (openSet[i].FCost < pathfindingNode.FCost || openSet[i].FCost == pathfindingNode.FCost)
                {
                    if (openSet[i].HCost < pathfindingNode.HCost)
                        pathfindingNode = openSet[i];
                }
            }

            openSet.Remove(pathfindingNode);
            closedSet.Add(pathfindingNode);

            if (pathfindingNode == targetPathfindingNode) 
            {
                var path = RetracePath(startPathfindingNode,targetPathfindingNode);
                return path.Select(n => n.WorldPosition).ToList();
            }

            foreach (PathfindingNode neighbour in GetNeighbours(pathfindingNode)) 
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour)) 
                {
                    continue;
                }

                int newCostToNeighbour = pathfindingNode.GCost + GetDistance(pathfindingNode, neighbour);
                if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour)) 
                {
                    neighbour.GCost = newCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetPathfindingNode);
                    neighbour.Parent = pathfindingNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return new List<Vector3Int>();
    }

    List<PathfindingNode> RetracePath(PathfindingNode startPathfindingNode, PathfindingNode endPathfindingNode) 
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode currentPathfindingNode = endPathfindingNode;

        while (currentPathfindingNode != startPathfindingNode)
        {
            path.Add(currentPathfindingNode);
            currentPathfindingNode = currentPathfindingNode.Parent;
        }

        path.Reverse();

        return path;
    }

    int GetDistance(PathfindingNode pathfindingNodeA, PathfindingNode pathfindingNodeB) 
    {
        int dstX = Mathf.Abs(pathfindingNodeA.GridX - pathfindingNodeB.GridX);
        int dstY = Mathf.Abs(pathfindingNodeA.GridY - pathfindingNodeB.GridY);

        if (dstX > dstY)
        {
            return 14*dstY + 10 * (dstX-dstY);
        }

        return 14*dstX + 10 * (dstY-dstX);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(_grid.cellSize.x * mapWidth, _grid.cellSize.y * mapHeight, 1f));
    }

    public void UpdateWalkability(Vector3Int gridPosition)
    {
        var node = PfNodeFromWorldCell(gridPosition);
        node.Walkable = _obstaclesTilemap.GetTile(gridPosition) == null;
    }
}

public enum MoveDirection
{
    Up,
    Right,
    Down,
    Left
}
