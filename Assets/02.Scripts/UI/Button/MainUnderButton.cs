using UnityEngine;
using UnityEngine.UI;

public class MainUnderButton : MonoBehaviour
{
    public Sprite passiveSprite;
    public Sprite activeSprite;
    public GameObject activeUI;
    public GameObject deco;

    private UIManager _uiManager;
    private Button _button;
    private Image _image;

    private void Awake()
    {
        _uiManager = UIManager.Instance;
        _button = this.gameObject.GetComponent<Button>();
        _image = _button.GetComponent<Image>();
        
        _button.onClick.AddListener(OnClickButton);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        _uiManager.ToggleUI(activeUI);
        ToggleDeco();
    }

    private void ToggleDeco()
    {
        bool isActive = activeUI.activeSelf;
        deco.SetActive(!isActive);
    }
}
