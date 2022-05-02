using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : TimelineTickable, IPlayerCharacterEffect
{
    [SerializeField] List<MoveSequenceElement> _moveSequence;
    [SerializeField] private Sprite _spriteLeft;
    [SerializeField] private Sprite _spriteTop;
    [SerializeField] private Sprite _spriteDown;

    private int _currentMoveSeqIndex;
    private int _currentMoveSeqProgress;
    private Vector3Int _initialPos;

    public Vector3Int PrevGridPosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _initialPos = GridPosition;
    }

    public override void Tick(float tickDuration)
    {
        PrevGridPosition = GridPosition;
        
        if (_currentMoveSeqProgress >= _moveSequence[_currentMoveSeqIndex].MoveLength)
        {
            NextMoveSeq();
            if (_currentMoveSeqIndex >= _moveSequence.Count)
            {
                ResetMoveSeq();
            }
        }
        
        var currentMove = _moveSequence[_currentMoveSeqIndex];
        var movePos = GridPosition + GridController.GetVectorInDirection(currentMove.Direction, 1);
        SetSprite(movePos - GridPosition);
        _currentMoveSeqProgress++;

        GridPosition = movePos;
        StartCoroutine(Move(tickDuration));
    }

    private void SetSprite(Vector3Int direction)
    {
        var r = GetComponentInChildren<SpriteRenderer>();
        r.flipX = false;
        if (direction.y > 0)
        {
            r.sprite = _spriteTop;
        }
        else if (direction.y < 0)
        {
            r.sprite = _spriteDown;
        }
        else
        {
            r.sprite = _spriteLeft;
            r.flipX = direction.x > 0;
        }
    }

    private void NextMoveSeq()
    {
        _currentMoveSeqIndex++;
        _currentMoveSeqProgress = 0;
    }

    private void ResetMoveSeq()
    {
        _currentMoveSeqProgress = 0;
        _currentMoveSeqIndex = 0;
        GridPosition = _initialPos;
        transform.position = Grid.CellToWorld(GridPosition);
    }

    private void OnDrawGizmos()
    {
        var grid = GetComponentInParent<GridController>();
        if (grid == null)
            return;
        
        Vector3Int currentCell = grid.WorldToCell(transform.position);
        Vector3 targetPos = transform.position;
        foreach (var moveSeqEl in _moveSequence)
        {
            Vector3 currentPos = grid.CellToWorld(currentCell);
            Vector3Int targetCell = currentCell
                                    + GridController.GetVectorInDirection(moveSeqEl.Direction, moveSeqEl.MoveLength);
            targetPos = grid.CellToWorld(targetCell);
        
            Gizmos.color = Color.green;
            Gizmos.DrawLine(currentPos, targetPos);
            Gizmos.DrawSphere(targetPos, 0.2f);

            currentCell = targetCell;
        }
        
        Gizmos.DrawLine(targetPos, transform.position);
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
}

[Serializable]
public class MoveSequenceElement
{
    public MoveDirection Direction;
    public int MoveLength;
}
