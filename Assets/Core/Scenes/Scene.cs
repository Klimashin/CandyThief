using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Scene : IScene 
{
    public SceneConfig SceneConfig { get; }
    public ComponentsBase<IRepository> RepositoriesBase { get; }
    public ComponentsBase<IInteractor> InteractorsBase { get; }
    
    public T GetSceneParam<T>(string key)
    {
        if (_sceneParams.ContainsKey(key) && _sceneParams[key] is T)
        {
            return (T) _sceneParams[key];
        }

        return default;
    }

    private readonly Dictionary<string, object> _sceneParams;
    public Scene(SceneConfig config, Dictionary<string, object> sceneParams = null) 
    {
        SceneConfig = config;
        _sceneParams = sceneParams ?? new Dictionary<string, object>(); 
        RepositoriesBase = new ComponentsBase<IRepository>(config.RepositoriesReferences);
        InteractorsBase = new ComponentsBase<IInteractor>(config.InteractorsReferences);
    }

    public void BuildUI()
    {
        UI.Build(SceneConfig);
    }
    
    public void SendMessageOnCreate() 
    {
        RepositoriesBase.SendMessageOnCreate();
        RepositoriesBase.SendMessageOnCreate();
        UI.controller.SendMessageOnCreate();
    }

    public Coroutine InitializeAsync()
    {
        return Coroutines.StartRoutine(InitializeAsyncRoutine());
    }

    private IEnumerator InitializeAsyncRoutine() 
    {
        yield return RepositoriesBase.InitializeAllComponents();
        yield return InteractorsBase.InitializeAllComponents();
        
        RepositoriesBase.SendMessageOnInitialize();
        InteractorsBase.SendMessageOnInitialize();
        UI.controller.SendMessageOnInitialize();
    }

    public void Start() 
    {
        RepositoriesBase.SendMessageOnStart();
        InteractorsBase.SendMessageOnStart();
        UI.controller.SendMessageOnStart();
    }

    public T GetRepository<T>() where T : IRepository 
    {
        return RepositoriesBase.GetComponent<T>();
    }

    public IEnumerable<T> GetRepositories<T>() where T : IRepository 
    {
        return RepositoriesBase.GetComponents<T>();
    }

    public T GetInteractor<T>() where T : IInteractor 
    {
        return InteractorsBase.GetComponent<T>();
    }
    
    public IEnumerable<T> GetInteractors<T>() where T : IInteractor 
    {
        return InteractorsBase.GetComponents<T>();
    }
}