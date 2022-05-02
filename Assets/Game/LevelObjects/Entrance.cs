using UnityEngine;

public class Entrance : TileTrigger
{
    [SerializeField] private AudioClip _winSfx;
    [SerializeField] private SoundSystem _soundSystem;
    
    public override void ActivateTrigger(PlayerCharacter character)
    {
        if (!character.HasCake)
        {
            return;
        }
        
        _soundSystem.PlayOneShot(_winSfx);

        Timeline.LevelCompleted();
    }

    protected override void OnDrawGizmos()
    {
        var grid = GetComponentInParent<GridController>();
        if (grid == null)
            return;

        var currentCell = grid.WorldToCell(transform.position);
        var currentCellPos = grid.CellToWorld(currentCell);
        
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawCube(currentCellPos, Vector3.one);
    }
}
