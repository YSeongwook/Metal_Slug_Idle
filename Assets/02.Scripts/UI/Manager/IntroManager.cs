using DG.Tweening;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class IntroManager : MonoBehaviour, IPointerDownHandler
{
    [TabGroup("Press to Start")] public TextMeshProUGUI startText; // TextMeshPro 텍스트 객체
    [TabGroup("Press to Start")] [SerializeField] private float textBlinkDuration = 1f; // 텍스트 블링크 지속 시간
    [TabGroup("Loading Settings")] [SerializeField] private float loadingUIDuration = 3f; // 로딩 UI 유지 시간

    private Tween _blinkingTween; // 텍스트 블링크 효과를 위한 Tween
    private bool _isFirebaseReady; // Firebase 준비 상태
    private bool _canStartGame; // 게임 시작 가능 여부

    private Logger logger;

    private void Awake()
    {
        logger = Logger.Instance;

        // 이벤트 리스너 등록
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 제거
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
    }

    // 텍스트를 깜박이기 시작하는 메서드
    private void StartBlinkingText()
    {
        startText.gameObject.SetActive(true);
        // DOTween을 사용하여 텍스트 알파값을 페이드 인/아웃
        _blinkingTween = startText.DOFade(0, textBlinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    // 화면 터치 또는 마우스 클릭을 감지하는 메서드
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_canStartGame) return;

        StartGame();
        startText.gameObject.SetActive(false);
    }

    // 게임을 시작하는 메서드
    private async void StartGame()
    {
        // DOTween 애니메이션 정지
        _blinkingTween.Kill();

        // Loading UI 활성화
        EventManager<UIEvents>.TriggerEvent(UIEvents.StartLoading);

        // 로딩 UI를 일정 시간 동안 유지
        await UniTask.Delay((int)(loadingUIDuration * 1000));

        // Loading UI 비활성화
        EventManager<UIEvents>.TriggerEvent(UIEvents.EndLoading);
    }

    // Firebase 인증 완료 시 호출되는 메서드
    private void OnFirebaseSignIn()
    {
        logger.Log("Firebase 초기화 완료");
        _isFirebaseReady = true;
        CheckIfReadyToStart();
    }

    // 게임을 시작할 준비가 되었는지 확인하는 메서드
    private void CheckIfReadyToStart()
    {
        if (!_isFirebaseReady) return;

        StartBlinkingText();
        _canStartGame = true;
    }
}
