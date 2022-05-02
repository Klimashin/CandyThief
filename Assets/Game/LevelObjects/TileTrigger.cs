using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class TileTrigger : MonoBehaviour
{
    public virtual bool TriggeredByIDeath { get; } = false;

    public Vector3Int GridPosition { get; protected set; }
    
    protected GridController Grid;
    protected GameTimeline Timeline;

    public void UpdateGridPosition()
    {
        GridPosition = Grid.WorldToCell(transform.position);
        transform.position = Grid.CellToWorld(GridPosition);
    }

    protected virtual void Awake()
    {
        Grid = GetComponentInParent<GridController>();
        Timeline = GetComponentInParent<GameTimeline>();
        UpdateGridPosition();
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

    public virtual void DeathTrigger(List<IDeath> triggerObjects)
    {
        foreach (var triggerObject in triggerObjects)
        {
            triggerObject.Death();
        }
    }
}
