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

        UnityMainThreadDispatcher.Enqueue(() => CreateLogMessage(message));
    }

    public void LogError(string message)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Logger: logMessagePrefab 또는 logContent가 설정되지 않았습니다.");
            return;
        }

        UnityMainThreadDispatcher.Enqueue(() => CreateLogMessage($"<color=red>{message}</color>"));
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