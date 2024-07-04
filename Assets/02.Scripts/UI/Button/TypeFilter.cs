using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.UI;

public class TypeFilter : MonoBehaviour
{
    [SerializeField] private Image typeButtonIcon; // 아이콘을 변경할 이미지
    public string heroType; // 버튼에 설정된 타입
    public Sprite[] typeSprites; // 타입에 따른 스프라이트 배열

    private Button _typeButton;
    private GameObject _parent;
    
    private void Awake()
    {
        _typeButton = GetComponent<Button>();
        _parent = gameObject.transform.parent.gameObject;
    }

    private void Start()
    {
        _typeButton.onClick.AddListener(OnTypeButtonClick);
    }

    private void OnDestroy()
    {
        _typeButton.onClick.RemoveListener(OnTypeButtonClick);
    }

    private void OnTypeButtonClick()
    {
        HeroDataManager.Instance.FilterHeroesByType(heroType);
        UpdateTypeIcon(heroType); // 아이콘 변경
        UIManager.Instance.ToggleUIWithoutMainClose(_parent);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickHeroTabButton);
    }

    private void UpdateTypeIcon(string type)
    {
        switch (type)
        {
            case "전체":
                typeButtonIcon.sprite = typeSprites[0];
                break;
            case "방어형":
                typeButtonIcon.sprite = typeSprites[1];
                break;
            case "근거리형":
                typeButtonIcon.sprite = typeSprites[2];
                break;
            case "원거리형":
                typeButtonIcon.sprite = typeSprites[3];
                break;
            case "만능형":
                typeButtonIcon.sprite = typeSprites[4];
                break;
            default:
                typeButtonIcon.sprite = typeSprites[0];
                break;
        }
    }
}