using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : ISceneManager
{
    public event SceneManagerHandler OnSceneLoadStartedEvent;
    public event SceneManagerHandler OnSceneLoadCompletedEvent;

    public Dictionary<string, SceneConfig> ScenesConfigMap { get; }
    public IScene CurrentScene { get; private set; }
    public bool IsLoading { get; private set; }

    public SceneManager() 
    {
        ScenesConfigMap = new Dictionary<string, SceneConfig>();
        InitializeSceneConfigs();
    }
    
    private const string CONFIG_FOLDER = "SceneConfigs";
    private void InitializeSceneConfigs() 
    {
        var allSceneConfigs = Resources.LoadAll<SceneConfig>(CONFIG_FOLDER);
        foreach (var sceneConfig in allSceneConfigs)
        {
            ScenesConfigMap[sceneConfig.SceneName] = sceneConfig;
        }
    }

    public Coroutine LoadScene(string sceneName, Dictionary<string, object> sceneParams = null) 
    {
        return LoadAndInitializeScene(sceneName, true, sceneParams);
    }
    
    public Coroutine InitializeCurrentScene(Dictionary<string, object> sceneParams = null) 
    {
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return LoadAndInitializeScene(sceneName, false, sceneParams);
    }

    private Coroutine LoadAndInitializeScene(string sceneName, bool loadNewScene, Dictionary<string, object> sceneParams = null) 
    {
        ScenesConfigMap.TryGetValue(sceneName, out var config);

        if (config == null)
        {
            throw new NullReferenceException($"There is no scene ({sceneName}) in the scenes list. The name is wrong or you forget to add it o the list.");
        }

        return Coroutines.StartRoutine(LoadSceneRoutine(config, loadNewScene, sceneParams));
    }

    private IEnumerator LoadSceneRoutine(SceneConfig config, bool loadNewScene, Dictionary<string, object> sceneParams) 
    {
        LoadingScreen.Instance.Show(this);
            
        IsLoading = true;
        OnSceneLoadStartedEvent?.Invoke(config);

        if (loadNewScene)
        {
            yield return Coroutines.StartRoutine(LoadSceneAsyncRoutine(config));
        }

        yield return Coroutines.StartRoutine(InitializeSceneRoutine(config, sceneParams));

        yield return new WaitForSecondsRealtime(0.1f);
        IsLoading = false;
        OnSceneLoadCompletedEvent?.Invoke(config);

        LoadingScreen.Instance.Hide(this);
    }

    private IEnumerator LoadSceneAsyncRoutine(SceneConfig config) 
    {
        var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(config.SceneName);
        asyncOperation.allowSceneActivation = false;

        var progressDivider = 0.9f;
        var progress = asyncOperation.progress / progressDivider;
        
        while (progress < 1f) 
        {
            yield return null;
            progress = asyncOperation.progress / progressDivider;
        }

        asyncOperation.allowSceneActivation = true;
    }

    private IEnumerator InitializeSceneRoutine(SceneConfig config, Dictionary<string, object> sceneParams) 
    {

        CurrentScene = new Scene(config, sceneParams);
        yield return null;

        CurrentScene.BuildUI();
        yield return null;

        CurrentScene.SendMessageOnCreate();
        yield return null;
        
        yield return CurrentScene.InitializeAsync();

        CurrentScene.Start();
    }
}