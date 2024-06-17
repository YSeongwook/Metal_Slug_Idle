using Cysharp.Threading.Tasks; // UniTask를 사용하기 위해 필요
using DG.Tweening;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [TabGroup("Press to Start")] public TextMeshProUGUI startText; // TextMeshPro 텍스트 객체
    [TabGroup("Press to Start")] [SerializeField] private float textBlinkDuration = 1f;

    [TabGroup("Load Next Scene"), Required] public string nextSceneName; // 다음 씬

    private Tween blinkingTween; // 텍스트 블링크 효과를 위한 Tween

    private bool isGPGSReady = false; // GPGS 준비 상태
    private bool isFirebaseReady = false; // Firebase 준비 상태

    private bool canStartGame = false; // 게임 시작 가능 여부

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

    private void Update()
    {
        // 터치 입력 감지
        if (canStartGame)
        {
            DetectTouch();
        }
    }

    private void StartBlinkingText()
    {
        startText.gameObject.SetActive(true);
        // DOTween을 사용하여 텍스트 알파값을 페이드 인/아웃
        blinkingTween = startText.DOFade(0, textBlinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void DetectTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 터치 시작 시 호출
                StartGame();
                startText.gameObject.SetActive(false);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭도 감지
            StartGame();
            startText.gameObject.SetActive(false);
        }
    }

    private async void StartGame()
    {
        // DOTween 애니메이션 정지
        blinkingTween.Kill();

        // Loading UI 활성화
        EventManager<UIEvents>.TriggerEvent(UIEvents.StartLoading);

        // 다음 씬을 비동기적으로 로드
        await LoadSceneAsync(nextSceneName);

        // Loading UI 비활성화
        EventManager<UIEvents>.TriggerEvent(UIEvents.EndLoading);

        // 이 오브젝트 비활성화
        this.gameObject.SetActive(false);
    }

    private async UniTask LoadSceneAsync(string sceneName)
    {
        var sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
        sceneLoadOperation.allowSceneActivation = false;

        while (!sceneLoadOperation.isDone)
        {
            // 로딩이 거의 완료되었을 때 씬 활성화
            if (sceneLoadOperation.progress >= 0.9f)
            {
                sceneLoadOperation.allowSceneActivation = true;
            }
            await UniTask.Yield();
        }
    }

    private void OnFirebaseSignIn()
    {
        logger.Log("Firebase 초기화 완료");
        isFirebaseReady = true;
        CheckIfReadyToStart();
    }

    private void CheckIfReadyToStart()
    {
        if (!isFirebaseReady) return;
        
        StartBlinkingText();
        canStartGame = true;
    }
}
