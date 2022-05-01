using System;
using DG.Tweening;
using UnityEngine;

public abstract class UIElement : MonoBehaviour, IUIElement
{
	[SerializeField] private float _fadeInOutDuration = 0f;
	
	public event Action<IUIElement> OnElementHideStartedEvent;
	public event Action<IUIElement> OnElementHiddenCompletelyEvent;
	public event Action<IUIElement> OnElementStartShowEvent;
	public event Action<IUIElement> OnElementShownEvent;
	public event Action<IUIElement> OnElementDestroyedEvent;

	public bool IsActive { get; protected set; } = true;
	protected UIController UIController => UI.controller;

	public virtual void Show() 
	{
		OnPreShow();
		gameObject.SetActive(true);
		IsActive = true;
		
		OnElementStartShowEvent?.Invoke(this);

		if (_fadeInOutDuration > 0f && TryGetComponent<CanvasGroup>(out var canvasGroup))
		{
			canvasGroup.alpha = 0f;
			canvasGroup
				.DOFade(1f, _fadeInOutDuration)
				.SetEase(Ease.InQuad)
				.SetUpdate(true)
				.OnComplete(OnPostShow);
		}
		else
		{
			OnPostShow();
		}
	}

	protected virtual void OnPreShow() { }

	protected virtual void OnPostShow()
	{
		OnElementShownEvent?.Invoke(this);
	}

	public void Hide() 
	{
		if (!IsActive)
		{
			return;
		}

		OnPreHide();

		OnElementHideStartedEvent?.Invoke(this);
		
		if (_fadeInOutDuration > 0f && TryGetComponent<CanvasGroup>(out var canvasGroup))
		{
			canvasGroup
				.DOFade(0f, _fadeInOutDuration)
				.SetEase(Ease.OutQuad)
				.SetUpdate(true)
				.OnComplete(HideInstantly);
		}
		else
		{
			HideInstantly();
		}
	}

	public virtual void HideInstantly() 
	{
		if (!IsActive)
		{
			return;
		}

		IsActive = false;
		gameObject.SetActive(false);
		OnPostHide();
		OnElementHiddenCompletelyEvent?.Invoke(this);
	}

	protected virtual void OnPreHide() { }
	protected virtual void OnPostHide() { }

	protected virtual void OnValidate()
	{
		if (_fadeInOutDuration > 0f && !TryGetComponent<CanvasGroup>(out var cg))
		{
			gameObject.AddComponent<CanvasGroup>();
		}
	}
}
