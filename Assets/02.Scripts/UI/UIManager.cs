using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Inspector Variables

    [FoldoutGroup("UIs")] public GameObject introUI;
    [FoldoutGroup("UIs")] public GameObject signInUI;
    [FoldoutGroup("UIs")] public GameObject loadingUI;
    [FoldoutGroup("UIs")] public GameObject mainUI;
    [FoldoutGroup("UIs")] public GameObject joystickUI;
    [FoldoutGroup("UIs")] public GameObject heroUI;
    [FoldoutGroup("UIs")] public GameObject inventoryUI;
    [FoldoutGroup("UIs")] public GameObject upgradeUI;
    [FoldoutGroup("UIs")] public GameObject contentsUI;
    [FoldoutGroup("UIs")] public GameObject questUI;
    [FoldoutGroup("UIs")] public GameObject shopUI;

    // Sign In UI
    [FoldoutGroup("Sign In UI")]
    [HorizontalGroup("Sign In UI/Horizontal")]
    [FoldoutGroup("Sign In UI/Horizontal/Buttons")]
    public GameObject signInButtons;

    [FoldoutGroup("Sign In UI/Horizontal/Buttons")]
    public Button signInGoogle;

    [FoldoutGroup("Sign In UI/Horizontal/Buttons")]
    public Button signInEmail;

    [FoldoutGroup("Sign In UI/Horizontal/Email Sign In")]
    public GameObject emailSignIn;

    [FoldoutGroup("Sign In UI/Horizontal/Email Sign In")]
    public TMP_InputField inputFieldID;

    [FoldoutGroup("Sign In UI/Horizontal/Email Sign In")]
    public TMP_InputField inputFieldPW;

    // Main UI
    [FoldoutGroup("Main UI")] public GameObject chatUI;

    [FoldoutGroup("Main UI/UpperBar")] public GameObject upperBar;
    [FoldoutGroup("Main UI/UpperBar")] public GameObject upperPower;
    [FoldoutGroup("Main UI/UpperBar")] public GameObject upperCard;
    [FoldoutGroup("Main UI/UpperBar")] public GameObject upperDeco;
    [FoldoutGroup("Main UI/UpperBar")] public TMP_Text textPower;
    [FoldoutGroup("Main UI/UpperBar")] public TMP_Text textCard;
    [FoldoutGroup("Main UI/UpperBar")] public TMP_Text textRuby;
    [FoldoutGroup("Main UI/UpperBar")] public TMP_Text textGold;

    [FoldoutGroup("Main UI/Buttons")] public Button menuButton;
    [FoldoutGroup("Main UI/Buttons")] public Button mainBossButton;
    [FoldoutGroup("Main UI/Buttons")] public Button settingBossButton;
    [FoldoutGroup("Main UI/Buttons")] public Button autoButtonOff;
    [FoldoutGroup("Main UI/Buttons")] public Button autoButtonOn;

    [HorizontalGroup("Main UI/Upper")] [FoldoutGroup("Main UI/Upper/Panel User Profile")]
    public GameObject userProfilePanel;

    [HorizontalGroup("Main UI/Upper")] [FoldoutGroup("Main UI/Upper/Panel Stage")]
    public GameObject stagePanel;

    [HorizontalGroup("Main UI/Horizontal")]
    [FoldoutGroup("Main UI/Horizontal/Panel Left Buttons")]
    [PropertySpace(0f, 5f)]
    public GameObject leftButtonsPanel;

    [FoldoutGroup("Main UI/Horizontal/Panel Left Buttons")] public Button getButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Left Buttons")] public Button camButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Left Buttons")] public Button chatButton;

    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] [PropertySpace(0f, 5f)]
    public GameObject mainUnderPanel;

    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public Button mainCloseButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public Button mainHeroButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public Button mainInventoryButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public Button mainUpgradeButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public Button mainContentsButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public Button mainQuestButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public Button mainShopButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Under Buttons")] public GameObject underDeco;

    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] [PropertySpace(0f, 5f)]
    public GameObject rightMenusPanel;

    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button menuCloseButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button rankingButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button settingButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button sleepButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button messageButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button noticeButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button crewButton;
    [FoldoutGroup("Main UI/Horizontal/Panel Right Menus")] public Button calendarButton;

    [FoldoutGroup("Main UI/Panel Party")] public GameObject partyPanel;

    [FoldoutGroup("Main UI/Panel Party")] [PropertySpace(5f, 0f)]
    public GameObject[] heroes;

    [FoldoutGroup("Shop UI")] public Button summonSingleButton;
    [FoldoutGroup("Shop UI")] public Button summonTenButton;
    [FoldoutGroup("Shop UI")] public Button summonThirtyButton;
    
    [FoldoutGroup("Log Scroll View")] public GameObject logScrollView;
    [FoldoutGroup("Log Scroll View")] public Button logButton;
    
    private bool _isActiveChatUI;
    private List<GameObject> activeUIs = new List<GameObject>();

    #endregion

    // 싱글톤 초기화 및 버튼 클릭 이벤트 등록
    protected override void Awake()
    {
        base.Awake();
        AddButtonEvents();
        AddInputFieldEvents(); // InputField 이벤트 등록
    }

    private void Start()
    {
        // 인트로 UI와 로그 버튼을 제외하고 자식 오브젝트 모두 비활성화
        foreach (Transform child in transform)
        {
            if (child.gameObject != introUI && child.gameObject != logButton.gameObject)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // 이벤트 리스너 제거
    private void OnDestroy()
    {
        RemoveButtonEvents();
        RemoveInputFieldEvents(); // InputField 이벤트 제거
    }
    
    private void AddButtonEvents()
    {
        menuButton.onClick.AddListener(() => EnableUI(rightMenusPanel));
        menuCloseButton.onClick.AddListener(() => DisableUI(rightMenusPanel));
    }

    private void RemoveButtonEvents()
    {
        menuButton.onClick.RemoveListener(() => EnableUI(rightMenusPanel));
        menuCloseButton.onClick.RemoveListener(() => DisableUI(rightMenusPanel));
    }

    // InputField의 onEndEdit 이벤트 리스너를 등록하는 메서드
    private void AddInputFieldEvents()
    {
        inputFieldID.onEndEdit.AddListener(OnEndEdit);
        inputFieldPW.onEndEdit.AddListener(OnEndEdit);
    }

    // InputField의 onEndEdit 이벤트 리스너를 제거하는 메서드
    private void RemoveInputFieldEvents()
    {
        inputFieldID.onEndEdit.RemoveListener(OnEndEdit);
        inputFieldPW.onEndEdit.RemoveListener(OnEndEdit);
    }

    #region Intro UI

    // 로그 UI를 토글하는 메서드
    public void ToggleLog()
    {
        logScrollView.SetActive(!logScrollView.activeSelf);
    }

    // 로그 창을 닫는 메서드
    public void ExitLog()
    {
        logScrollView.SetActive(false);
        signInUI.SetActive(false);
    }

    // 수동 Google Sign-In 이벤트를 트리거하는 메서드
    public void ManualGoogleSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickManualGPGSSignIn);
    }

    // 이메일 로그인 UI를 활성화하는 메서드
    public void EnableEmailSignInUI()
    {
        signInButtons.SetActive(false);
        emailSignIn.SetActive(true);
    }

    // 로딩 UI를 표시하는 메서드
    public void ShowLoadingUI()
    {
        loadingUI.SetActive(true);
    }

    // 로딩 UI를 숨기는 메서드
    public void HideLoadingUI()
    {
        loadingUI.SetActive(false);
        introUI.SetActive(false);
        mainUI.SetActive(true);
        joystickUI.SetActive(true);
    }

    // 로그인 UI를 활성화하는 메서드
    public void EnableSignInUI()
    {
        signInUI.SetActive(true);
    }

    // 로그인 UI를 비활성화하는 메서드
    public void DisableSignInUI()
    {
        signInUI.SetActive(false);
    }

    // 이메일 로그인 UI를 비활성화하는 메서드
    public void DisableEmailSignInUI()
    {
        signInUI.SetActive(false);
    }

    // 인트로 UI를 비활성화하는 메서드
    public void DisableIntroUI()
    {
        introUI.SetActive(false);
    }

    // InputField의 onEndEdit 이벤트 핸들러
    private void OnEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CloseSoftKeyboard();
        }
    }

    #endregion

    #region Main UI

    // 특정 UI 오브젝트를 활성화
    public void EnableUI(GameObject uiObject)
    {
        if (activeUIs.Contains(uiObject)) return;
        
        uiObject.SetActive(true);
        activeUIs.Add(uiObject);
    }

    // 특정 UI 오브젝트를 비활성화
    public void DisableUI(GameObject uiObject)
    {
        if(activeUIs.Count == 0) return;
        if (!activeUIs.Contains(uiObject)) return;
        
        uiObject.SetActive(false);
        activeUIs.Remove(uiObject);
    }
    
    // 특정 UI 오브젝트를 토글하는 메서드
    public void ToggleUI(GameObject uiObject)
    {
        if (activeUIs.Contains(uiObject))
        {
            // 이미 활성화된 UI가 다시 눌리면 비활성화
            uiObject.SetActive(false);
            activeUIs.Remove(uiObject);
            if (activeUIs.Count == 0)
            {
                mainCloseButton.gameObject.SetActive(false);
            }
        }
        else
        {
            // 다른 UI가 활성화되어 있으면 모두 비활성화
            CloseAllUIs();
            // 새로운 UI 활성화
            uiObject.SetActive(true);
            activeUIs.Add(uiObject);
            mainCloseButton.gameObject.SetActive(true);
        }
    }

    // 현재 활성화된 모든 UI를 비활성화하는 메서드
    public void CloseAllUIs()
    {
        foreach (var ui in activeUIs)
        {
            ui.SetActive(false);
        }
        activeUIs.Clear();
        mainCloseButton.gameObject.SetActive(false);
        underDeco.SetActive(true);
    }
    
    // 메인 하단 버튼들 토글
    public void ToggleMainUnderButtons()
    {
        bool isActive = mainBossButton.gameObject.activeSelf;
        
        mainHeroButton.gameObject.SetActive(!isActive);
        mainInventoryButton.gameObject.SetActive(!isActive);
        mainUpgradeButton.gameObject.SetActive(!isActive);
        mainContentsButton.gameObject.SetActive(!isActive);
        mainQuestButton.gameObject.SetActive(!isActive);
        mainShopButton.gameObject.SetActive(!isActive);
    }
    
    public void EnableMainCloseButton()
    {
        mainCloseButton.gameObject.SetActive(true);
        underDeco.SetActive(false);
    }

    // 채팅 UI를 토글하는 메서드
    public void ToggleChatUI()
    {
        chatUI.SetActive(!chatUI.activeSelf);
    }

    #endregion

    #region Shop UI

    public void OnClickSummonButton()
    {
        // 10회뽑, 30회뽑, 자동 소환 버튼 활성화

        ToggleMainUnderButtons(); // 하단 버튼들 토글
        EnableMainCloseButton(); // 하단 닫기 버튼 활성화, 하단 데코 비활성화
        upperDeco.SetActive(false); // 상단바 데코 비활성화
    }

    #endregion

    #region Upper Bar Position and Visibility

    // upperBar의 위치를 변경하는 메서드
    public void SetUpperBarPosition(Vector3 position)
    {
        upperBar.transform.localPosition = position;
    }

    // ShopUI가 활성화될 때 upperCard와 upperPower의 가시성을 변경하는 메서드
    public void OnShopUIEnable()
    {
        upperPower.SetActive(false);
        upperCard.SetActive(true);
    }

    // ShopUI가 비활성화될 때 upperCard와 upperPower의 가시성을 변경하는 메서드
    public void OnShopUIDisable()
    {
        upperPower.SetActive(true);
        upperCard.SetActive(false);
    }

    #endregion

    // 소프트 키보드를 닫는 메서드
    private void CloseSoftKeyboard()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (TouchScreenKeyboard.visible)
        {
            TouchScreenKeyboard.hideInput = true;
        }
    }

    // 활성화된 UI 리스트에서 특정 UI 오브젝트 제거
    public void RemoveUIFromList(GameObject uiObject)
    {
        if (activeUIs.Contains(uiObject))
        {
            activeUIs.Remove(uiObject);
        }
    
        if (activeUIs.Count == 0)
        {
            mainCloseButton.gameObject.SetActive(false);
        }
    }

}
