using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class TimelineTickable : SerializedMonoBehaviour
{
    public Vector3Int GridPosition { get; protected set; }
    
    public virtual int TickOrder => 1;

    public abstract void Tick(float tickDuration);
    
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
        Timeline.AddObject(this);
    }

    protected virtual void OnDisable()
    {
        Timeline.RemoveObject(this);
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
    
    protected IEnumerator Move(float tickDuration)
    {
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = Grid.CellToWorld(GridPosition);
        float timePassed = 0f;

        while (tickDuration > timePassed)
        {
            timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, timePassed / tickDuration);
            yield return null;
        }
    }
}
