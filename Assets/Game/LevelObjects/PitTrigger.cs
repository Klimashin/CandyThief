using System.Collections.Generic;
using UnityEngine;

public class PitTrigger : TileTrigger
{
    [SerializeField] private AudioClip _fallSfx;
    [SerializeField] private SoundSystem _soundSystem;
    
    public override bool TriggeredByIDeath { get; } = true;

    public override void ActivateTrigger(PlayerCharacter character)
    {
        
    }

    public override void DeathTrigger(List<IDeath> triggerObjects)
    {
        if (triggerObjects.Count > 0)
            _soundSystem.PlayOneShot(_fallSfx);
        
        foreach (var triggerObject in triggerObjects)
        {
            triggerObject.Death(DeathType.Fall);
        }
    }
    
    protected virtual void OnDrawGizmos()
    {
        var grid = GetComponentInParent<GridController>();
        if (grid == null)
            return;

        var currentCell = grid.WorldToCell(transform.position);
        var currentCellPos = grid.CellToWorld(currentCell);
        
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(currentCellPos, Vector3.one);
    }
}
