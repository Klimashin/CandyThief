using System;
using System.Collections;
using System.IO;
using System.Threading;
using Sirenix.Serialization;
using UnityEngine;


public sealed class FileStorage
{
	public GameData GameData { get; private set; }
	
	public event Action OnStorageSaveStartedEvent;
	public event Action OnStorageSaveCompleteEvent;
	public event Action<GameData> OnStorageLoadedEvent;
	
	public string FilePath { get; }
	
	public FileStorage(string fileName = "TEST") 
	{
		var folder = "Saves";
		var folderPath = $"{Application.persistentDataPath}/{folder}";
		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}

		FilePath = $"{folderPath}/{fileName}";
	}
	
	public void Save() 
	{
		OnStorageSaveStartedEvent?.Invoke();
		SaveInternal();
		OnStorageSaveCompleteEvent?.Invoke();
	}

	public void SaveAsync(Action callback = null)
	{
		OnStorageSaveStartedEvent?.Invoke();
		SaveAsyncInternal(callback);
		OnStorageSaveCompleteEvent?.Invoke();
	}

	public Coroutine SaveWithRoutine(Action callback = null)
	{
		OnStorageSaveStartedEvent?.Invoke();
		return SaveWithRoutineInternal(() => {
			callback?.Invoke();
			OnStorageSaveCompleteEvent?.Invoke();
		});
	}

	public void Load() 
	{
		LoadInternal();
		OnStorageLoadedEvent?.Invoke(GameData);
	}

	public void LoadAsync(Action<GameData> callback = null) 
	{
		LoadAsyncInternal(loadedData => {
			callback?.Invoke(GameData);
			OnStorageLoadedEvent?.Invoke(GameData);
		});
	}

	public Coroutine LoadWithRoutine(Action<GameData> callback = null) 
	{
		return LoadWithRoutineInternal(loadedData => {
			callback?.Invoke(GameData);
			OnStorageLoadedEvent?.Invoke(GameData);
		});
	}

	public T Get<T>(string key) 
	{
		return GameData.Get<T>(key);
	}
	
	public T Get<T>(string key, T valueByDefault) 
	{
		return GameData.Get(key, valueByDefault);
	}

	public void Set<T>(string key, T value) 
	{
		GameData.Set(key, value);
	}

	public override string ToString() 
	{
		return GameData.ToString();
	}

	private void SaveInternal() 
	{
		var path = Path.Combine(FilePath);
		var bytes = SerializationUtility.SerializeValue(GameData, DataFormat.Binary);
		File.WriteAllBytes(path, bytes);
	}

	private void SaveAsyncInternal(Action callback = null) 
	{
		var thread = new Thread(() => SaveDataTaskThreaded(callback));
		thread.Start();
	}
	
	private void SaveDataTaskThreaded(Action callback) 
	{
		Save();
		callback?.Invoke();
	}

	private Coroutine SaveWithRoutineInternal(Action callback = null) 
	{
		return Coroutines.StartRoutine(SaveRoutine(callback));
	}
	
	private IEnumerator SaveRoutine(Action callback) 
	{
		var threadEnded = false;
		
		SaveAsync(() => {
			threadEnded = true;
		});
		
		while (!threadEnded)
			yield return null;
		
		callback?.Invoke();
	}

	private void LoadInternal() 
	{
		if (!File.Exists(FilePath)) 
		{
			var gameDataByDefault = new GameData();
			GameData = gameDataByDefault;
			Save();
		}
		
		var bytes = File.ReadAllBytes(FilePath);
		GameData = SerializationUtility.DeserializeValue<GameData>(bytes, DataFormat.Binary);
	}
	
	private void LoadAsyncInternal(Action<GameData> callback = null) 
	{
		var thread = new Thread(() => LoadDataTaskThreaded(callback));
		thread.Start();
	}
	
	private void LoadDataTaskThreaded(Action<GameData> callback) 
	{
		Load();
		callback?.Invoke(GameData);
	}

	private Coroutine LoadWithRoutineInternal(Action<GameData> callback = null) 
	{
		return Coroutines.StartRoutine(LoadRoutine(callback));
	}
	
	private IEnumerator LoadRoutine(Action<GameData> callback) 
	{
		var threadEnded = false;
		var gameData = new GameData();
		
		LoadAsync((loadedData) => {
			threadEnded = true;
		});
		
		while (!threadEnded)
			yield return null;
		
		callback?.Invoke(gameData);
	}
}
