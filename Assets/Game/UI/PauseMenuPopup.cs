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
        Game.InputActions.Pause.Close.performed += OnUnPauseAction;
    }

    private void OnDisable()
    {
        Game.InputActions.Pause.Close.performed -= OnUnPauseAction;
        _mainMenuButton.onClick.RemoveListener(MainMenuButtonClick);
        Time.timeScale = 1f;
    }

    protected override void OnPreShow()
    {
        base.OnPreShow();
        Time.timeScale = 0f;
        Game.InputActions.Gameplay.Disable();
        Game.InputActions.Pause.Enable();
    }

    protected override void OnPostHide()
    {
        base.OnPostHide();
        Time.timeScale = 1f;
        Game.InputActions.Gameplay.Enable();
        Game.InputActions.Pause.Disable();
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
}
