using System.Collections.Generic;
using UnityEngine;

public class HunterEnemy : TimelineTickable, IDeath, IPlayerCharacterEffect
{
    public override void Tick(float tickDuration)
    {
        var path = Grid.FindPath(GridPosition, Timeline.PlayerCharacter.GridPosition);
        if (path.Count == 0)
        {
            return;
        }
        
        GridPosition = path[0];
        
        StartCoroutine(Move(tickDuration));
    }

    public void Death()
    {
        gameObject.SetActive(false);
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
