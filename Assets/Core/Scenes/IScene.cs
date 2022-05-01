using System.Collections.Generic;
using UnityEngine;

public interface IScene 
{
    
    SceneConfig SceneConfig { get; }
    ComponentsBase<IRepository> RepositoriesBase { get; }
    ComponentsBase<IInteractor> InteractorsBase { get; }

    T GetSceneParam<T>(string key);


    void BuildUI();
    void SendMessageOnCreate();
    Coroutine InitializeAsync();
    void Start();

    T GetRepository<T>() where T : IRepository;
    IEnumerable<T> GetRepositories<T>() where T : IRepository;
    
    T GetInteractor<T>() where T : IInteractor;
    IEnumerable<T> GetInteractors<T>() where T : IInteractor;
}