using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayScreen : UIScreen
{
    [SerializeField] private SoundSystem _soundSystem;
    
    private PauseMenuPopup _pauseMenu;

    public override void OnCreate()
    {
        base.OnCreate();
        _soundSystem.PlayMusicClip(Game.SceneManager.CurrentScene.SceneConfig.SceneMusic);
    }

    protected override void OnPostShow()
    {
        base.OnPostShow();
        
        _pauseMenu = UIController.GetUIElement<PauseMenuPopup>();
        
        Game.InputActions.Gameplay.Pause.performed += OnPauseAction;

        _pauseMenu.OnElementStartShowEvent += OnPauseMenuStartShow;
        _pauseMenu.OnElementHiddenCompletelyEvent += OnPauseMenuHiddenCompletely;
    }

    private void OnPauseMenuHiddenCompletely(IUIElement obj)
    {
        Game.InputActions.Gameplay.Pause.performed += OnPauseAction;
    }
    
    private void OnPauseMenuStartShow(IUIElement pauseMenu)
    {
        Game.InputActions.Gameplay.Pause.performed -= OnPauseAction;
    }

    private void OnPauseAction(InputAction.CallbackContext context)
    {
        if (!_pauseMenu.IsActive)
            _pauseMenu.Show();
    }

    private void OnDestroy()
    {
        Game.InputActions.Gameplay.Pause.performed -= OnPauseAction;
    }
}
