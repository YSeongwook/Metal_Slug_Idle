using UnityEngine;
using UnityEngine.UI;

public class SubUI : MonoBehaviour
{
    public Button button;
    public Sprite passiveSprite;
    public Sprite activeSprite;
    
    private void OnEnable()
    {
        button.GetComponent<Image>().sprite = activeSprite;
    }

    private void OnDisable()
    {
        button.GetComponent<Image>().sprite = passiveSprite;
    }
}