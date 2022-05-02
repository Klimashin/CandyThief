using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorDestroyTrigger : TileTrigger
{
    [SerializeField] private PitTrigger _pit;
    [SerializeField] private TileBase _preDestroyedTile;
    [SerializeField] private AudioClip _crack1Sfx;
    [SerializeField] private AudioClip _crack2Sfx;
    [SerializeField] private SoundSystem _soundSystem;
    
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
        
        _soundSystem.PlayOneShot(_crack1Sfx);

        var currentTick = Timeline.CurrentTick;
        while (currentTick == Timeline.CurrentTick)
        {
            yield return null;
        }
        
        _soundSystem.PlayOneShot(_crack2Sfx);
        
        Grid.GetFloorTilemap().SetTile(GridPosition, null);
        var pit = Instantiate(_pit, Grid.transform);
        pit.transform.position = transform.position;
        pit.UpdateGridPosition();

        var takedown = Timeline
            .GetObjectsInATile(GridPosition)
            .OfType<IDeath>()
            .ToList();
        
        pit.DeathTrigger(takedown);

        enabled = false;
    }
}
