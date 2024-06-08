using System.Collections.Generic;
using UnityEngine;

public static class QueueExtensions
{
    // 오브젝트 풀에 오브젝트를 추가하는 확장 메서드
    public static void EnqueuePool<T>(this Queue<T> queue, T item) where T : Component
    {
        item.gameObject.SetActive(false);
        queue.Enqueue(item);
    }

    // 오브젝트 풀에서 오브젝트를 가져오는 확장 메서드
    public static T DequeuePool<T>(this Queue<T> queue) where T : Component
    {
        if (queue.Count == 0)
        {
            Debug.LogWarning("Queue.Count == 0");
            return null;
        }

        T item = queue.Dequeue();
        item.gameObject.SetActive(true);
        return item;
    }
}

// 풀 관리 클래스
public class Pool
{
    public Queue<Component> queue;
    public int count;
    public Transform parentTransform;

    public Pool(Transform parentTransform)
    {
        queue = new Queue<Component>();
        count = 0;
        this.parentTransform = parentTransform;
    }
}

// 오브젝트를 풀링 형식으로 사용할 수 있도록 도와주는 클래스
public class ObjectPool : MonoBehaviour
{
    // 오브젝트 풀 딕셔너리
    private Dictionary<string, Pool> objectPools = new Dictionary<string, Pool>();

    // 풀을 count만큼 생성
    public void CreatePool(GameObject prefab, int count = 100)
    {
        string itemType = prefab.name;
        if (!objectPools.ContainsKey(itemType)) // 키가 없을 경우
        {
            // 풀을 생성할 트랜스폼을 추가하고, 생성한 풀은 딕셔너리에 추가
            GameObject poolContainer = new GameObject(itemType + "Pool");
            poolContainer.transform.SetParent(this.transform);
            objectPools.Add(itemType, new Pool(poolContainer.transform));
        }

        for (int i = 0; i < count; i++) // count만큼 프리팹 오브젝트를 생성해서 인큐
        {
            GameObject item = Instantiate(prefab, objectPools[itemType].parentTransform);
            item.name = itemType;
            objectPools[itemType].queue.EnqueuePool(item.GetComponent<Component>());
            objectPools[itemType].count++;
        }
    }

    // 사용한 오브젝트를 큐에 다시 인큐, Destroy를 대체
    public void EnqueueObject(GameObject item)
    {
        string itemType = item.name;
        if (!objectPools.ContainsKey(itemType)) // 키가 없는 경우
        {
            CreatePool(item);   // 자동으로 풀을 생성
        }
        item.transform.SetParent(objectPools[itemType].parentTransform);
        objectPools[itemType].queue.EnqueuePool(item.GetComponent<Component>());
    }

    // prefab과 같은 타입의 모든 오브젝트를 큐에 다시 인큐
    public void AllDestroyObject(GameObject prefab)
    {
        string itemType = prefab.name;
        if (!objectPools.ContainsKey(itemType)) // 키가 없는 경우
        {
            CreatePool(prefab); // 자동으로 풀을 생성
        }

        for (int i = 0; i < objectPools[itemType].parentTransform.childCount; i++) // 키 값에 해당하는 트랜스폼의 모든 아이템 검사
        {
            GameObject item = objectPools[itemType].parentTransform.GetChild(i).gameObject;
            if (item.activeSelf)
            {
                EnqueueObject(item); // 활성화되어있을 경우 디큐
            }
        }
    }

    // 사용할 오브젝트를 반환 Instantiate를 대체
    public GameObject DequeueObject(GameObject prefab)
    {
        string itemType = prefab.name;
        if (!objectPools.ContainsKey(itemType)) // 키가 없는 경우
        {
            CreatePool(prefab); // 자동으로 풀을 생성
        }

        Component dequeuedObject = objectPools[itemType].queue.DequeuePool();

        // 디큐 시도, 큐에 있는 모든 아이템이 사용 중일 경우 null을 반환
        if (dequeuedObject != null) // 큐에 오브젝트가 있을 경우
        {
            return dequeuedObject.gameObject; // 해당 오브젝트를 반환
        }
        else // 큐에 내용물이 없는 경우
        {
            CreatePool(prefab, objectPools[itemType].count); // 풀 확장
            return DequeueObject(prefab); // 추가한 풀에서 디큐
        }
    }
}
