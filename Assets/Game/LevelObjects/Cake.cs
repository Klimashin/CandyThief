using DG.Tweening;
using UnityEngine;

public class Cake : TileTrigger
{
    [SerializeField] private SpriteRenderer _cakeRenderer;
    
    private bool _executed;
    public override void ActivateTrigger(PlayerCharacter character)
    {
        if (_executed)
        {
            return;
        }

        _executed = true;
        
        _cakeRenderer.transform
            .DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                character.CollectCake();
                enabled = false;
            });
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
