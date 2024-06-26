using UnityEngine;
using UnityEngine.UI;

public class SubUI : MonoBehaviour
{
    public Button button;
    public Sprite passiveSprite;
    public Sprite activeSprite;

    private Image _buttonImage;

    private void Awake()
    {
        _buttonImage = button.GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (button != null && activeSprite != null)
        {
            _buttonImage.sprite = activeSprite;
        }
    }

    private void OnDisable()
    {
        if (button != null && passiveSprite != null)
        {
            _buttonImage.sprite = passiveSprite;
        }
    }
}