using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircularDotLoadingBar : MonoBehaviour
{
    public GameObject dotPrefab; // 점 프리팹
    public int numberOfDots = 12; // 점의 개수
    public float radius = 100f; // 원의 반지름
    public float fadeDuration = 1f; // fade out 시간
    public float dotSpawnInterval = 0.1f; // 점 생성 간격
    public float rotationSpeed = 100f; // 회전 속도

    private bool isLoading = false;
    private Transform[] dotTransforms;

    private void Start()
    {
        dotTransforms = new Transform[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            float angle = i * Mathf.PI * 2f / numberOfDots;
            dot.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            dot.SetActive(false);
            dotTransforms[i] = dot.transform;
        }
    }

    private IEnumerator LoadingRoutine()
    {
        int currentDotIndex = 0;

        while (isLoading)
        {
            Transform dotTransform = dotTransforms[currentDotIndex];
            dotTransform.gameObject.SetActive(true);
            StartCoroutine(FadeOut(dotTransform.GetComponent<Image>()));

            currentDotIndex = (currentDotIndex + 1) % numberOfDots;
            yield return new WaitForSeconds(dotSpawnInterval);
        }
    }

    private IEnumerator FadeOut(Image image)
    {
        Color originalColor = image.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - normalizedTime);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        image.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isLoading)
        {
            transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime); // 시계 방향 회전
            Debug.Log("로딩 슬라이더 회전 중"); // 로깅: 로딩 슬라이더 회전 중
        }
    }

    public void StartLoading()
    {
        isLoading = true;
        StartCoroutine(LoadingRoutine());
    }

    public void StopLoading()
    {
        isLoading = false;
        StopAllCoroutines();
        foreach (Transform dotTransform in dotTransforms)
        {
            dotTransform.gameObject.SetActive(false);
        }
    }
}
