using UnityEngine;
using TMPro;

public class Logger : Singleton<Logger>
{
    public TMP_Text logMessagePrefab; // 프리팹으로 사용할 TextMeshPro 텍스트 객체
    public Transform logContent; // Content 오브젝트
    public int initialPoolSize = 10; // 초기 풀 크기
    
    private ObjectPool objectPool;
    private bool isInitialized;

    protected override void Awake()
    {
        base.Awake();
        // 추가 초기화 작업이 있으면 여기에 작성
        
        // Unity Engine의 null 체크를 최적화 하기 위해 ReferenceEquals를 사용하는게 좋지만
        // Public으로 선언한 오브젝트가 할당되지 않으면 fake null 상태가 되기 때문에
        // isInitialized 플래그를 사용해서 null 체크를 최적화함.
        isInitialized = logMessagePrefab != null && logContent != null;
        
        objectPool = gameObject.AddComponent<ObjectPool>();
        objectPool.CreatePool(logMessagePrefab.gameObject, initialPoolSize);
    }

    public void Log(string message)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Logger: logMessagePrefab 또는 logContent가 설정되지 않았습니다.");
            return;
        }

        CreateLogMessage(message);
        Debug.Log(message);
    }

    public void LogError(string message)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Logger: logMessagePrefab 또는 logContent가 설정되지 않았습니다.");
            return;
        }

        CreateLogMessage($"<color=red>{message}</color>");
        Debug.LogError(message);
    }

    private void CreateLogMessage(string message)
    {
        GameObject logTextObject = objectPool.DequeueObject(logMessagePrefab.gameObject);
        TMP_Text logText = logTextObject.GetComponent<TMP_Text>();
        logText.transform.SetParent(logContent, false);
        logText.text = message;
    }

    public void ReturnLogMessage(TMP_Text logText)
    {
        objectPool.EnqueueObject(logText.gameObject);
    }
}