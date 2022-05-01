using System.Collections;
using  UnityEngine;

public class Coroutines : MonoBehaviour 
{

    #region CONSTANTS

    private const string NAME = "[COROUTINE MANAGER]";

    #endregion
        
    private static Coroutines Instance => GetInstance();
    private static Coroutines _instance;
    private static bool IsInitialized => _instance != null;
        

    private static Coroutines GetInstance() 
    {
        if (!IsInitialized)
            _instance = CreateSingleton();
        return _instance;
    }

    private static Coroutines CreateSingleton() 
    {
        var createdManager = new GameObject(NAME).AddComponent<Coroutines>();
        createdManager.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(createdManager.gameObject);
        return createdManager;
    }

    public static Coroutine StartRoutine(IEnumerator enumerator) 
    {
        return Instance.StartCoroutine(enumerator);
    }

    public static void StopRoutine(Coroutine routine) 
    {
        if (routine != null)
            Instance.StopCoroutine(routine);
    }

    public static void StopRoutine(string routineName) 
    {
        Instance.StopCoroutine(routineName);
    }
}