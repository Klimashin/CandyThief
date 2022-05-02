using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HunterEnemy : TimelineTickable, IDeath, IPlayerCharacterEffect
{
    public int AggroRadius = 5;
    private Animator _animator;
    
    public Vector3Int PrevGridPosition { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    public override void Tick(float tickDuration)
    {
        var path = Grid.FindPath(GridPosition, Timeline.PlayerCharacter.PrevGridPosition);
        if (path.Count == 0 || path.Count > AggroRadius)
        {
            return;
        }

        var direction = path[0] - GridPosition;
        _animator.SetBool(IsMoving, true);
        _animator.SetFloat(X, direction.x);
        _animator.SetFloat(Y, direction.y);

        PrevGridPosition = GridPosition;
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
        if (
            character.GridPosition == GridPosition 
            || character.GridPosition == PrevGridPosition && GridPosition == character.PrevGridPosition
        ) {
            character.Death();
        }
    }

    public List<Vector3Int> EffectTiles()
    {
        return new () { GridPosition, PrevGridPosition };
    }
    
    protected override IEnumerator Move(float tickDuration)
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
        
        _animator.SetBool(IsMoving, false);
    }
}
