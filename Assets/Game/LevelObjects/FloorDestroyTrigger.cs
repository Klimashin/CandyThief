using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorDestroyTrigger : TileTrigger
{
    [SerializeField] private TileBase _destroyedTile;
    [SerializeField] private TileBase _preDestroyedTile;

    private int _currentTilesListIndex = 0;
    private Coroutine _activationCoroutine;
    
    public override void ActivateTrigger(PlayerCharacter character)
    {
        if (_activationCoroutine != null)
        {
            return;
        }

        _activationCoroutine = StartCoroutine(TileDestroyCoroutine());
    }

    private IEnumerator TileDestroyCoroutine()
    {
        Grid.GetFloorTilemap().SetTile(GridPosition, _preDestroyedTile);

        var currentTick = Timeline.CurrentTick;
        while (currentTick == Timeline.CurrentTick)
        {
            yield return null;
        }

        Grid.GetFloorTilemap().SetTile(GridPosition, null);
        Grid.GetObstaclesTilemap().SetTile(GridPosition, _destroyedTile);

        var takedown = Timeline
            .GetObjectsInATile(GridPosition)
            .OfType<IDeath>();
        
        foreach (var death in takedown)
        {
            death.Death();
        }

        Grid.UpdateWalkability(GridPosition);

        enabled = false;
    }
}
