using System;
using UnityEngine;

public interface IArchitectureComponent : IArchitectureCaptureEvents 
{
	event Action OnInitializedEvent;

	ArchitectureComponentState state { get; }
	bool isInitialized { get; }
	bool isLoggingEnabled { get; set; }

	Coroutine InitializeWithRoutine();
}
