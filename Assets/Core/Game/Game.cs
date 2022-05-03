using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Game 
{
    public static event Action OnGameInitializedEvent;

    public static ArchitectureComponentState state { get; private set; } = ArchitectureComponentState.NotInitialized;
    public static bool isInitialized => state == ArchitectureComponentState.Initialized;
    public static ISceneManager SceneManager { get; private set; }
    public static InputActions InputActions { get; private set; }
   // public static FileStorage FileStorage { get; private set; }

    public static void Run() 
    {
        Coroutines.StartRoutine(RunGameRoutine());
    }

    private static IEnumerator RunGameRoutine() 
    {
        state = ArchitectureComponentState.Initializing;
        InputActions = new InputActions();
        //FileStorage = new FileStorage();
        SceneManager = new SceneManager();

        yield return null;
        
        yield return SceneManager.InitializeCurrentScene();

        state = ArchitectureComponentState.Initialized;
        OnGameInitializedEvent?.Invoke();
    }

    public static T GetInteractor<T>() where T : IInteractor 
    {
        return SceneManager.CurrentScene.GetInteractor<T>();
    }

    public static IEnumerable<T> GetInteractors<T>() where T : IInteractor 
    {
        return SceneManager.CurrentScene.GetInteractors<T>();
    }

    public static T GetRepository<T>() where T : IRepository 
    {
        return SceneManager.CurrentScene.GetRepository<T>();
    }
    
    public static IEnumerable<T> GetRepositories<T>() where T : IRepository 
    {
        return SceneManager.CurrentScene.GetRepositories<T>();
    }
}