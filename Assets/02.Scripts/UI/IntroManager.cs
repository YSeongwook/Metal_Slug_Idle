using DG.Tweening;
using EnumTypes;
using EventLibrary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Blink Start Text")]
    public TextMeshProUGUI startText; // TextMeshPro 텍스트 객체
    [SerializeField] private float textBlinkDuration = 1f;

    [Header("Load Next Scene")]
    public string nextSceneName; // 다음 씬 이름

    private Tween blinkingTween; // 텍스트 블링크 효과를 위한 Tween

    private bool isGPGSReady = false; // GPGS 준비 상태
    private bool isFirebaseReady = false; // Firebase 준비 상태

    private Logger logger;

    private void Awake()
    {
        logger = Logger.Instance;
        
        // 이벤트 리스너 등록
        EventManager<GoogleEvents>.StartListening(GoogleEvents.GPGSSignIn, OnGPGSSignIn);
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
    }

    private void Start()
    {
        // 텍스트 반짝임 시작
        StartBlinkingText();
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 제거
        EventManager<GoogleEvents>.StopListening(GoogleEvents.GPGSSignIn, OnGPGSSignIn);
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
    }

    private void Update()
    {
        // 터치 입력 감지
        DetectTouch();
    }

    private void StartBlinkingText()
    {
        // DOTween을 사용하여 텍스트 알파값을 페이드 인/아웃
        blinkingTween = startText.DOFade(0, textBlinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void DetectTouch()
    {
        if (isGPGSReady && isFirebaseReady)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    // 터치 시작 시 호출
                    StartGame();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                // 마우스 클릭도 감지
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        // DOTween 애니메이션 정지
        blinkingTween.Kill();

        // 다음 씬 또는 상태로 전환
        SceneManager.LoadScene(nextSceneName);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickStart);
        this.gameObject.SetActive(false);
    }
    
    private void OnGPGSSignIn()
    {
        logger.Log("GPGS 로그인 완료");
        isGPGSReady = true;
    }

    private void OnFirebaseSignIn()
    {
        logger.Log("Firebase 초기화 완료");
        isFirebaseReady = true;
    }
}
