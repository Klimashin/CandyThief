using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class GameTimeline : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private float _tickDuration = 0.5f;
    
    private readonly Queue<PlayerActionType> _actionsQueue = new ();
    private readonly HashSet<TimelineTickable> _timelineObjects = new ();
    private readonly HashSet<TileTrigger> _tileTriggers = new ();

    public int CurrentTick { get; private set; }
    public PlayerCharacter PlayerCharacter => _playerCharacter;
    
    private void Start()
    {
        StartCoroutine(TimelineCoroutine());
    }

    public void EnqueuePlayerAction(PlayerActionType actionType)
    {
        _actionsQueue.Enqueue(actionType);
    }

    private IEnumerator TimelineCoroutine()
    {
        while (true)
        {
            if (_actionsQueue.Count > 0)
            {
                var action = _actionsQueue.Dequeue();
                _playerCharacter.PlanAction(action);
                
                foreach (var timelineTickable in _timelineObjects.OrderBy(obj => obj.TickOrder))
                {
                    timelineTickable.Tick(_tickDuration);
                }

                float timePassed = 0f;
                while (_tickDuration/2f > timePassed)
                {
                    timePassed += Time.deltaTime;
                    yield return null;
                }

                CurrentTick++;
                ApplyTileTriggers();
                
                while (_tickDuration > timePassed)
                {
                    timePassed += Time.deltaTime;
                    yield return null;
                }

                ApplyPlayerCharacterEffects();

                if (_playerCharacter.IsDead)
                {
                    break;
                }

                continue;
            }
            
            yield return null;
        }

        if (_playerCharacter.IsDead)
        {
            yield return _playerCharacter.DeathAnimation();

            Game.SceneManager.LoadScene("Gameplay");
        }
    }

    private void ApplyTileTriggers()
    {
        foreach (var tileTrigger in _tileTriggers)
        {
            if (tileTrigger.GridPosition == _playerCharacter.GridPosition)
            {
                tileTrigger.ActivateTrigger(_playerCharacter);
            }
        }
    }

    private void ApplyPlayerCharacterEffects()
    {
        _timelineObjects
            .Where(o => 
                o is IPlayerCharacterEffect effect && 
                effect.EffectTiles().Contains(_playerCharacter.GridPosition))
            .Select(o => o as IPlayerCharacterEffect)
            .ForEach(e => e.ApplyEffect(_playerCharacter));
    }

    public void AddObject(TimelineTickable timelineTickable)
    {
        _timelineObjects.Add(timelineTickable);
    }

    public void RemoveObject(TimelineTickable timelineTickable)
    {
        _timelineObjects.Remove(timelineTickable);
    }

    public void RemoveTileTrigger(TileTrigger tileTrigger)
    {
        _tileTriggers.Remove(tileTrigger);
    }

    public void AddTileTrigger(TileTrigger tileTrigger)
    {
        _tileTriggers.Add(tileTrigger);
    }
    
    public List<TimelineTickable> GetObjectsInATile(Vector3Int gridPos)
    {
        return _timelineObjects.Where(obj => obj.GridPosition == gridPos).ToList();
    }
}

public interface IPlayerCharacterEffect
{
    void ApplyEffect(PlayerCharacter character);

    List<Vector3Int> EffectTiles();
}
