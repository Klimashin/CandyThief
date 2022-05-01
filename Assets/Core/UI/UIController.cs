using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class UIController : MonoBehaviour 
{
	public event Action OnUIBuiltEvent;

	[SerializeField] private Camera _uiCamera;
	[SerializeField] private UILayer[] _layers;

	public Camera UICamera => _uiCamera;
	public bool IsUIBuilt { get; private set; }
	public bool IsLoggingEnabled { get; set; }


	private Dictionary<Type, IUIElementOnLayer> _createdUIElementsMap;
	private Dictionary<Type, UIPopup> _cachedPopupsMap;
	private List<Type> _uiDynamicPrefabTypes;
	private SceneConfig _sceneConfig;

	private void Awake() 
	{
		_createdUIElementsMap = new Dictionary<Type, IUIElementOnLayer>();
		_uiDynamicPrefabTypes = new List<Type>();
		_cachedPopupsMap = new Dictionary<Type, UIPopup>();

		DontDestroyOnLoad(gameObject);
	}

	public void SendMessageOnCreate()
	{
		var allCreatedElements = _createdUIElementsMap.Values.ToArray();
		foreach (var element in allCreatedElements)
			element.OnCreate();
	}
	
	public void SendMessageOnInitialize() 
	{
		var allCreatedElements = _createdUIElementsMap.Values.ToArray();
		foreach (var element in allCreatedElements)
			element.OnInitialize();
	}
	
	public void SendMessageOnStart() 
	{
		var allCreatedElements = _createdUIElementsMap.Values.ToArray();
		foreach (var element in allCreatedElements)
			element.OnStart();
	}
	

	public T ShowUIElement<T>() where T : UIElement, IUIElementOnLayer 
	{
		var type = typeof(T);

		if (_createdUIElementsMap.TryGetValue(type, out var foundElement))
		{
			foundElement.Show();
			return (T) foundElement;
		}

		_cachedPopupsMap.TryGetValue(type, out var cachedPopup);
		if (cachedPopup != null) 
		{
			cachedPopup.Show();
			return cachedPopup as T;
		}

		var prefab = _sceneConfig.GetPrefab(type);
		return CreateAndShowElement<T>(prefab);
	}

	private T CreateAndShowElement<T>(IUIElementOnLayer prefab) where T : UIElement, IUIElementOnLayer 
	{
		var container = GetContainer(prefab.layer);
		var createdElementGo = Instantiate(prefab.gameObject, container);
		createdElementGo.name = prefab.name;
		var createdElement = createdElementGo.GetComponent<T>();
		var type = typeof(T);

		_createdUIElementsMap[type] = createdElement;
		createdElement.Show();
		createdElement.OnElementHiddenCompletelyEvent += OnElementHiddenCompletely;
		return createdElement;
	}

	private void OnElementHiddenCompletely(IUIElement uiElement) 
	{
		if (uiElement is IUIPopup uiPopup && uiPopup.isPreCached)
		{
			return;
		}

		var type = uiElement.GetType();
		uiElement.OnElementHiddenCompletelyEvent -= OnElementHiddenCompletely;
		_createdUIElementsMap.Remove(type);
	}

	public void BuildUI(SceneConfig sceneConfig) 
	{
		_sceneConfig = sceneConfig;

		var prefabs = _sceneConfig.GetUIPrefabs();
		foreach (var uiElementPref in prefabs) 
		{
			if (uiElementPref is UIScreen uiScreenPref)
			{
				if (uiScreenPref.showByDefault)
				{
					CreateAndShowElement<UIScreen>(uiScreenPref);
				}
				else
				{
					CreateHiddenScreen(uiScreenPref);
				}
				
				continue;
			}

			if (uiElementPref is UIPopup popupPref && popupPref.isPreCached)
			{
				CreateCachedPopup(popupPref);
			} else {
				RememberTypeForLaterCreation(uiElementPref);
			}
		}

		IsUIBuilt = true;
		
		if (IsLoggingEnabled) 
		{
			Debug.Log($"INTERFACE CREATED SUCCESSFULLY: " +
			          $"total elements: {prefabs.Length}, " +
			          $"created: {_createdUIElementsMap.Count}, " +
			          $"pre cached popups: {_cachedPopupsMap.Count}");
		}

		Resources.UnloadUnusedAssets();

		SetupOverlayCamera();
		
		OnUIBuiltEvent?.Invoke();
	}

	private void SetupOverlayCamera()
	{
		var mainCamera = Camera.main;
		var cameraData = mainCamera.GetUniversalAdditionalCameraData();
		cameraData.cameraStack.Add(_uiCamera);
	}

	private void CreateCachedPopup(UIPopup popupPref) 
	{
		var container = GetContainer(popupPref.layer);
		var createdCachedPopup = Instantiate(popupPref, container);
		createdCachedPopup.name = popupPref.name;
		var type = createdCachedPopup.GetType();

		_cachedPopupsMap[type] = createdCachedPopup;
		_createdUIElementsMap[type] = createdCachedPopup;

		createdCachedPopup.HideInstantly();
	}

	private void CreateHiddenScreen(UIScreen uiScreenPref) 
	{
		var container = GetContainer(uiScreenPref.layer);
		var createdUIScreen = Instantiate(uiScreenPref, container);
		createdUIScreen.name = uiScreenPref.name;
		var type = createdUIScreen.GetType();
		_createdUIElementsMap[type] = createdUIScreen;
		createdUIScreen.HideInstantly();
	}

	private Transform GetContainer(UILayerType layer) 
	{
		return _layers.First(layerObject => layerObject.layer == layer).transform;
	}

	private void RememberTypeForLaterCreation(IUIElement uiElementPref) 
	{
		var type = uiElementPref.GetType();
		_uiDynamicPrefabTypes.Add(type);
	}

	public T GetUIElement<T>() where T : UIElement 
	{
		var type = typeof(T);
		_createdUIElementsMap.TryGetValue(type, out var uiElement);
		return (T) uiElement;
	}

	public void Clear() 
	{
		if (_createdUIElementsMap == null)
			return;

		var allCreatedUIElements = _createdUIElementsMap.Values.ToArray();
		foreach (var uiElement in allCreatedUIElements)
			Destroy(uiElement.gameObject);

		_createdUIElementsMap.Clear();
		_cachedPopupsMap.Clear();
		_uiDynamicPrefabTypes.Clear();
	}


#if UNITY_EDITOR
	private void Reset() 
	{
		if (_uiCamera == null)
		{
			_uiCamera = GetComponentInChildren<Camera>();
		}

		_layers = GetComponentsInChildren<UILayer>();
	}
#endif
	
}