using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "Architecture/Scenes/New SceneConfig")]
public sealed class SceneConfig : ScriptableObject
{
    [SerializeField, SceneName] private string _sceneName;

    [Header("======= CORE ARCHITECTURE =======")]
    [SerializeField, ClassReference(typeof(IRepository))]
    private string[] _repositoryReferences;
    
    [SerializeField, ClassReference(typeof(IInteractor))]
    private string[] _interactorsReferences;

    
    [Header("======= UI STRUCTURE ======="), Space (20)]
    [SerializeField]
    private List<GameObject> _uiPrefabs;

    public string SceneName => _sceneName;
    public string[] RepositoriesReferences => _repositoryReferences;
    public string[] InteractorsReferences => _interactorsReferences;

    public IUIElementOnLayer[] GetUIPrefabs()
    {
        var uiPrefabs = new List<IUIElementOnLayer>();
        foreach (var goPrefab in _uiPrefabs) 
        {
            var uiPrefab = goPrefab.GetComponent<IUIElementOnLayer>();
            uiPrefabs.Add(uiPrefab);
        }

        return uiPrefabs.ToArray();
    }
    
    public IUIElementOnLayer GetPrefab(Type type)
    {
        var allPrefab = GetUIPrefabs();
        return allPrefab.First(pref => pref.GetType() == type);
    }

}