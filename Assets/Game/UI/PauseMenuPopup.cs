using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuPopup : UIPopup
{
    [SerializeField, SceneName] private string _mainMenuSceneName;
    [SerializeField] private Button _mainMenuButton;

    private void OnEnable()
    {
        _mainMenuButton.onClick.AddListener(MainMenuButtonClick);
    }

    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(MainMenuButtonClick);
        Time.timeScale = 1f;
    }

    protected override void OnPreShow()
    {
        base.OnPreShow();
        Time.timeScale = 0f;
    }

    protected override void OnPostShow()
    {
        base.OnPostShow();
        Game.InputActions.Gameplay.Pause.performed += OnUnPauseAction;
    }

    protected override void OnPreHide()
    {
        base.OnPreHide();
        Game.InputActions.Gameplay.Pause.performed -= OnUnPauseAction;
    }

    protected override void OnPostHide()
    {
        base.OnPostHide();
        Time.timeScale = 1f;
    }

    private void MainMenuButtonClick()
    {
        Game.SceneManager.LoadScene(_mainMenuSceneName);
    }
    
    private void OnUnPauseAction(InputAction.CallbackContext context)
    {
        if (IsActive)
        {
            Hide();
        }
    }

    private void OnDestroy()
    {
        Game.InputActions.Gameplay.Pause.performed -= OnUnPauseAction;
    }
}
