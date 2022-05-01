using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void SceneManagerHandler(SceneConfig config);
    
public interface ISceneManager 
{
    event SceneManagerHandler OnSceneLoadStartedEvent;
    event SceneManagerHandler OnSceneLoadCompletedEvent;

    IScene CurrentScene { get; }
    Dictionary<string, SceneConfig> ScenesConfigMap { get; }

    Coroutine LoadScene(string sceneName, Dictionary<string, object> sceneParams = null);
    Coroutine InitializeCurrentScene(Dictionary<string, object> sceneParams = null);
}