using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : UIScreen
{
    [SerializeField, SceneName] private string _gameplaySceneName;
    [SerializeField] private SoundSystem _soundSystem;
    
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;

    protected override void OnPostShow()
    {
        base.OnPostShow();
        
        SetMenuButtonsActive(true);
    }

    protected override void OnPreShow()
    {
        base.OnPreShow();
        _soundSystem.PlayMusicClip(Game.SceneManager.CurrentScene.SceneConfig.SceneMusic);
        SetMenuButtonsActive(false);
    }

    private void SetMenuButtonsActive(bool buttonsActive)
    {
        _startGameButton.gameObject.SetActive(buttonsActive);
        _settingsButton.gameObject.SetActive(buttonsActive);
        _quitButton.gameObject.SetActive(buttonsActive);
    }

    private void OnEnable()
    {
        Time.timeScale = 1f;
        _startGameButton.onClick.AddListener(OnStartButtonClick);
        _settingsButton.onClick.AddListener(OnSettingsButtonClick);
        _quitButton.onClick.AddListener(OnQuitButtonClick);
    }
    
    private void OnDisable()
    {
        _startGameButton.onClick.RemoveListener(OnStartButtonClick);
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
        _quitButton.onClick.RemoveListener(OnQuitButtonClick);
    }

    private void OnStartButtonClick()
    {
        Game.SceneManager.LoadScene(_gameplaySceneName);
    }

    private void OnSettingsButtonClick()
    {
        UIController.ShowUIElement<SettingsPopup>();
    }

    private void OnQuitButtonClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
