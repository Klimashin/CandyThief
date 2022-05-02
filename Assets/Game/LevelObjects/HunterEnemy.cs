using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HunterEnemy : TimelineTickable, IDeath, IPlayerCharacterEffect
{
    public int AggroRadius = 5;
    
    public override void Tick(float tickDuration)
    {
        var path = Grid.FindPath(GridPosition, Timeline.PlayerCharacter.PrevGridPosition);
        if (path.Count == 0 || path.Count > AggroRadius)
        {
            return;
        }
        
        GridPosition = path[0];
        
        StartCoroutine(Move(tickDuration));
    }

    public void Death(DeathType deathType = DeathType.Default)
    {
        enabled = false;
        if (deathType == DeathType.Default)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform
                .DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }

    public void ApplyEffect(PlayerCharacter character)
    {
        character.Death();
    }

    public List<Vector3Int> EffectTiles()
    {
        return new () { GridPosition };
    }
}
