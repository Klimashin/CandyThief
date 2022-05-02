using Sirenix.OdinInspector;
using UnityEngine;

public abstract class TileTrigger : MonoBehaviour
{
    public bool TriggeredByEnemy;
    
    public Vector3Int GridPosition { get; protected set; }
    
    protected GridController Grid;
    protected GameTimeline Timeline;

    protected virtual void Awake()
    {
        Grid = GetComponentInParent<GridController>();
        Timeline = GetComponentInParent<GameTimeline>();
        GridPosition = Grid.WorldToCell(transform.position);
        transform.position = Grid.CellToWorld(GridPosition);
    }
    
    protected virtual void OnEnable()
    {
        Timeline.AddTileTrigger(this);
    }

    protected virtual void OnDisable()
    {
        Timeline.RemoveTileTrigger(this);
    }

    [Button("Align on grid")]
    protected virtual void OnValidate()
    {
        var grid = GetComponentInParent<GridController>();
        if (grid == null)
            return;

        var currentCell = grid.WorldToCell(transform.position);
        transform.position = grid.CellToWorld(currentCell);
    }

    protected virtual void OnDrawGizmos()
    {
        var grid = GetComponentInParent<GridController>();
        if (grid == null)
            return;

        var currentCell = grid.WorldToCell(transform.position);
        var currentCellPos = grid.CellToWorld(currentCell);
        
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        Gizmos.DrawCube(currentCellPos, Vector3.one);
    }

    public abstract void ActivateTrigger(PlayerCharacter character);
}
