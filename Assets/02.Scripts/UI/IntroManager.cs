using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IntroManager : MonoBehaviour
{
    [Header("Blink Start Text")]
    public TextMeshProUGUI startText; // TextMeshPro 텍스트 객체
    [SerializeField] private float textBlinkDuration = 1f;
    
    [Header("Load Next Scene")]
    public string nextSceneName; // 다음 씬 이름

    private void Start()
    {
        // 텍스트 반짝임 시작
        StartBlinkingText();
    }

    private void Update()
    {
        // 터치 입력 감지
        DetectTouch();
    }

    private void StartBlinkingText()
    {
        // DOTween을 사용하여 텍스트 알파값을 페이드 인/아웃
        startText.DOFade(0, textBlinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
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
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭도 감지
            StartGame();
        }
    }

    private void StartGame()
    {
        // 다음 씬 또는 상태로 전환
        Debug.Log("Game Started!");
        SceneManager.LoadScene(nextSceneName);
    }
}