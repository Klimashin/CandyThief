using UnityEngine;
using UnityEngine.UI;

public class GameCompletedScreen : UIScreen
{
    [SerializeField, SceneName] private string _mainMenuSceneName;
    [SerializeField] private SoundSystem _soundSystem;
    
    [SerializeField] private Button _mainMenuButton;

    protected override void OnPostShow()
    {
        base.OnPostShow();
        _mainMenuButton.gameObject.SetActive(true);
    }

    protected override void OnPreShow()
    {
        base.OnPreShow();
        _soundSystem.PlayMusicClip(Game.SceneManager.CurrentScene.SceneConfig.SceneMusic);
        _mainMenuButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Time.timeScale = 1f;
        _mainMenuButton.onClick.AddListener(OnStartButtonClick);
    }
    
    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(OnStartButtonClick);
    }

    private void OnStartButtonClick()
    {
        Game.SceneManager.LoadScene(_mainMenuSceneName);
    }
}
