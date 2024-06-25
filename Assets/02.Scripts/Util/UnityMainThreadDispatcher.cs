using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    // 메인 스레드에서 실행될 작업들을 저장할 큐를 선언
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    // Unity의 Update 메서드는 매 프레임마다 호출됩니다.
    private void Update()
    {
        // 큐에 있는 작업들을 하나씩 실행
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                // 큐에서 작업을 꺼내서 실행
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    // 메인 스레드에서 실행할 작업을 큐에 추가
    public static void Enqueue(Action action)
    {
        if (action == null)
            throw new ArgumentNullException("action"); // action이 null이면 예외

        lock (_executionQueue)
        {
            // 큐에 작업을 추가
            _executionQueue.Enqueue(action);
        }
    }
}