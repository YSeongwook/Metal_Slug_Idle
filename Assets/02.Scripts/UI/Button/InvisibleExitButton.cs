using UnityEngine;
using UnityEngine.UI;

public class InvisibleExitButton : MonoBehaviour
{
    public GameObject deco;
    
    private Button exitButton;

    private void Awake()
    {
        // 버튼 컴포넌트를 가져와서 클릭 이벤트 리스너를 등록
        exitButton = GetComponent<Button>();
        
        if (exitButton == null) return;
        exitButton.onClick.AddListener(DisableUI);
    }

    private void OnDestroy()
    {
        // 객체가 파괴될 때 클릭 이벤트 리스너를 제거
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(DisableUI);
        }
    }

    private void DisableUI()
    {
        // 부모 UI를 비활성화하고 UIManager의 UI 리스트를 업데이트
        GameObject parentUI = transform.parent.gameObject;
        parentUI.SetActive(false);
        UIManager.Instance.RemoveUIFromList(parentUI);

        if (deco == null) return; 
        deco.SetActive(true);
    }
}