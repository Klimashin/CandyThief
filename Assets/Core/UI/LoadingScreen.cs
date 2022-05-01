using System;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private const string PREF_PATH = "[LOADING_SCREEN]";

    public event Action<object, LoadingScreen> OnLoadingScreenShownEvent;
    public event Action<object, LoadingScreen> OnLoadingScreenHideStartEvent;
    public event Action<object, LoadingScreen> OnLoadingScreenHiddenCompletelyEvent;

    public static LoadingScreen Instance 
    {
        get 
        {
            if (_instance != null)
            {
                return _instance;
            }

            var prefab = Resources.Load<LoadingScreen>(PREF_PATH);
            _instance = Instantiate(prefab);
            Resources.UnloadUnusedAssets();
            DontDestroyOnLoad(_instance.gameObject);

            return _instance;
        }
    }

    public static bool IsActive => _instance.gameObject.activeInHierarchy;

    
    private static LoadingScreen _instance;
    
    public void Show(object sender) 
    {
        gameObject.SetActive(true);
        OnLoadingScreenShownEvent?.Invoke(sender, this);
    }
    
    public void Hide(object sender) 
    {
        OnLoadingScreenHideStartEvent?.Invoke(sender, this);
        HideInstantly(sender);
    }
    
    public void HideInstantly(object sender)
    {
        gameObject.SetActive(false);
        OnLoadingScreenHiddenCompletelyEvent?.Invoke(sender, this);
    }
    
}