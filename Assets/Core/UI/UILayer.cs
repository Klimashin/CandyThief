using UnityEngine;

public sealed class UILayer : UIElement 
{

	[SerializeField] private UILayerType _layer;

	public UILayerType layer => _layer;
	
}

public enum UILayerType {
	Screen,
	FX_UnderPopups,
	Popup,
	FX_OverPopups
}