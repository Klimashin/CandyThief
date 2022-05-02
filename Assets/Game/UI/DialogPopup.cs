using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogPopup : UIPopup
{
    [SerializeField] private Image _imageLeft;
    [SerializeField] private Image _imageRight;
    [SerializeField] private TextMeshProUGUI _dialogText;

    private InputActions _inputActions;
    
    private void OnEnable()
    {
        _imageLeft.enabled = false;
        _imageRight.enabled = false;
        _dialogText.text = "";
    }

    public void ShowDialog(DialogData data)
    {
        _inputActions = Game.InputActions;
        
        Show();
        
        StartCoroutine(DialogCoroutine(data));
    }

    private IEnumerator DialogCoroutine(DialogData data)
    {
        _inputActions.Dialog.Enable();
        
        foreach (var replica in data.Texts)
        {
            var skipped = false;
            _imageLeft.enabled = replica.Character == Character.Main;
            _imageRight.enabled = replica.Character != Character.Main;
            _dialogText.text = replica.Text;
            
            while (!skipped)
            {
                skipped = _inputActions.Dialog.Skip.WasPerformedThisFrame();
                
                yield return null;
            }
        }

        yield return null;
        
        _inputActions.Dialog.Disable();

        Hide();
    }
}
