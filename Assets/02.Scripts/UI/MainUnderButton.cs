using UnityEngine;
using UnityEngine.UI;

public class MainUnderButton : MonoBehaviour
{
    public Sprite passiveSprite;
    public Sprite activeSprite;
    public GameObject activeUI;

    private UIManager _uiManager;
    private Button _button;
    private Image _image;
    private bool _isActive;

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
        // 스프라이트 변경
    }

    private void SwitchSprite()
    {
        bool isActive = activeUI.activeSelf;

        _image.sprite = isActive ? activeSprite : passiveSprite;
    }
}
