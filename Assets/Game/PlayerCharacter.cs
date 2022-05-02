using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerCharacter : TimelineTickable, IDeath
{
    [SerializeField] private Animator _animator;
    
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Y = Animator.StringToHash("Y");

    public bool IsDead { get; set; }
    public bool HasCake { get; private set; }
    public Vector3Int PrevGridPosition { get; private set; }

    public override int TickOrder => 0;

    public void PlanAction(PlayerActionType action)
    {
        _plannedAction = action;
    }

    public void Death(DeathType deathType = DeathType.Default)
    {
        IsDead = true;
        if (deathType == DeathType.Default)
        {
            StartCoroutine(DeathAnimation());
        }
        else
        {
            transform
                .DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InQuad);
        }
    }

    private PlayerActionType _plannedAction;
    private Coroutine _actionCoroutine;
    public override void Tick(float tickDuration)
    {
        if (_actionCoroutine != null)
        {
            StopCoroutine(_actionCoroutine);
        }
        
        _actionCoroutine = StartCoroutine(MoveAction(_plannedAction, tickDuration));
    }

    private IEnumerator MoveAction(PlayerActionType action, float tickDuration)
    {
        PrevGridPosition = GridPosition;
        
        Vector3Int moveDirection;
        switch (action)
        {
            case PlayerActionType.MoveDown:
                moveDirection = new Vector3Int(0, -1, 0);
                break;
            
            case PlayerActionType.MoveLeft:
                moveDirection = new Vector3Int(-1, 0, 0);
                break;
            
            case PlayerActionType.MoveUp:
                moveDirection = new Vector3Int(0, 1, 0);
                break;
            
            case PlayerActionType.MoveRight:
                moveDirection = new Vector3Int(1, 0, 0);
                break;
            
            default:
                throw new Exception("Unknown action");
        }
        
        _animator.SetBool(IsMoving, true);
        _animator.SetFloat(X, moveDirection.x);
        _animator.SetFloat(Y, moveDirection.y);
        
        Vector3Int tileToMoveTo = GridPosition + moveDirection;

        if (!Grid.IsCellFree(tileToMoveTo))
        {
            yield return AnimateInvalidMove(moveDirection, tickDuration);
            _animator.SetBool(IsMoving, false);
            yield break;
        }
        
        GridPosition = tileToMoveTo;

        yield return Move(tickDuration);
        
        _animator.SetBool(IsMoving, false);
    }

    private IEnumerator AnimateInvalidMove(Vector3 direction, float tickDuration)
    {
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = transform.position + direction / 3f;
        float timePassed = 0f;
        
        while (tickDuration/2f > timePassed)
        {
            timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, timePassed / tickDuration);
            yield return null;
        }
        
        while (tickDuration > timePassed)
        {
            timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(targetPosition, initialPosition, timePassed / tickDuration);
            yield return null;
        }
    }

    public IEnumerator DeathAnimation()
    {
        yield return new WaitForSeconds(1f);
    }

    public void CollectCake()
    {
        HasCake = true;
    }
}

public interface IDeath
{
    void Death(DeathType deathType = DeathType.Default);
}

public enum DeathType
{
    Default,
    Fall
}
