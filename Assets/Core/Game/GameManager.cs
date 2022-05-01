using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnApplicationPausedEvent;
    public static event Action OnApplicationUnpausedEvent;
    public static event Action OnApplicationFocusedEvent;
    public static event Action OnApplicationUnfocusedEvent;
    public static event Action OnApplicationQuitEvent;
    
    [Space, SerializeField] private bool isLoggingEnabled;

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        
        Game.Run();

        if (isLoggingEnabled)
        {
            Debug.Log($"GAME MANAGER: Game launched: {Application.productName}");
        }
    }

    private void OnApplicationPause(bool pauseStatus) 
    {
        if (pauseStatus) {
            if (isLoggingEnabled)
                Debug.Log("GAME MANAGER: Paused");

            OnApplicationPausedEvent?.Invoke();
        }
        else {
            if (isLoggingEnabled)
                Debug.Log("GAME MANAGER: Game unpaused");
            
            OnApplicationUnpausedEvent?.Invoke();
        }
    }

    private void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus) {
            if (isLoggingEnabled)
                Debug.Log("GAME MANAGER: Game focused");

            OnApplicationUnfocusedEvent?.Invoke();
        }
        else {
            if (isLoggingEnabled)
                Debug.Log("GAME MANAGER: Game unfocused");
            
            OnApplicationFocusedEvent?.Invoke();
        }
    }

    private void OnApplicationQuit() {
        if (isLoggingEnabled)
            Debug.Log("GAME MANAGER: Game exited");

        OnApplicationQuitEvent?.Invoke();
    }
}