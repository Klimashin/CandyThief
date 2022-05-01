using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private GameTimeline _timeline;
    
    private InputActions _input;

    private void Start()
    {
        Game.SceneManager.OnSceneLoadCompletedEvent += OnSceneLoadCompleted;
    }
    private void OnDisable()
    {
        Game.SceneManager.OnSceneLoadCompletedEvent -= OnSceneLoadCompleted;
    }
    
    private void OnSceneLoadCompleted(SceneConfig config)
    {
        _input = Game.InputActions;
        _input.Gameplay.Down.performed += MoveDown;
        _input.Gameplay.Left.performed += MoveLeft;
        _input.Gameplay.Up.performed += MoveUp;
        _input.Gameplay.Right.performed += MoveRight;
    }

    private void MoveRight(InputAction.CallbackContext obj) 
        => _timeline.EnqueuePlayerAction(PlayerActionType.MoveRight);
    private void MoveUp(InputAction.CallbackContext obj) 
        => _timeline.EnqueuePlayerAction(PlayerActionType.MoveUp);
    private void MoveLeft(InputAction.CallbackContext obj) 
        => _timeline.EnqueuePlayerAction(PlayerActionType.MoveLeft);
    private void MoveDown(InputAction.CallbackContext obj) 
        => _timeline.EnqueuePlayerAction(PlayerActionType.MoveDown);
}

public enum PlayerActionType
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight
}
