using UnityEngine.UI;


public interface IUIPopup : IUIElementOnLayer 
{
	bool isPreCached { get; }
	Button[] buttonsClose { get; }
}