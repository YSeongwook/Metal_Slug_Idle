using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using UnityEngine.Networking;

public class GachaManager : MonoBehaviour
{
    private string gachaUrl = "https://us-central1-unifire-ebcc1.cloudfunctions.net/gacha"; // 가챠 API의 URL
    public SummonResultManager summonResultManager; // SummonResultManager 컴포넌트 참조
    public HeroCollectionManager heroCollectionManager; // HeroCollectionManager 컴포넌트 참조

    private void OnEnable()
    {
        EventManager<GachaEvents>.StartListening(GachaEvents.GachaSingle, () => PerformGacha(1));
        EventManager<GachaEvents>.StartListening(GachaEvents.GachaTen, () => PerformGacha(10));
        EventManager<GachaEvents>.StartListening(GachaEvents.GachaThirty, () => PerformGacha(30));
    }

    private void OnDisable()
    {
        EventManager<GachaEvents>.StopListening(GachaEvents.GachaSingle, () => PerformGacha(1));
        EventManager<GachaEvents>.StopListening(GachaEvents.GachaTen, () => PerformGacha(10));
        EventManager<GachaEvents>.StopListening(GachaEvents.GachaThirty, () => PerformGacha(30));
    }

    public void PerformGacha(int drawCount)
    {
        string userId = AuthManager.Instance.GetCurrentUser().UserId; // 현재 사용자 ID 가져오기
        StartCoroutine(GachaCoroutine(userId, drawCount)); // 가챠 코루틴 실행
    }

    private IEnumerator GachaCoroutine(string userId, int drawCount)
    {
        var json = JsonConvert.SerializeObject(new { userId = userId, drawCount = drawCount }); // 요청 데이터를 JSON으로 직렬화
        var request = new UnityWebRequest(gachaUrl, "POST"); // POST 요청 생성
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json); // 요청 바디 설정
        request.uploadHandler = new UploadHandlerRaw(bodyRaw); // 업로드 핸들러 설정
        request.downloadHandler = new DownloadHandlerBuffer(); // 다운로드 핸들러 설정
        request.SetRequestHeader("Content-Type", "application/json"); // 요청 헤더 설정

        yield return request.SendWebRequest(); // 요청 전송 및 응답 대기

        if (request.result == UnityWebRequest.Result.Success)
        {
            var result = JsonConvert.DeserializeObject<GachaResult>(request.downloadHandler.text); // 응답 데이터 파싱
            UpdateUIWithGachaResult(result.result); // UI 업데이트
            UpdateHeroCollection(result.result); // HeroCollection 업데이트
        }
        else
        {
            Debug.LogError("Gacha request failed: " + request.error); // 에러 처리
        }
    }

    private void UpdateUIWithGachaResult(int[] heroIds)
    {
        Dictionary<int, int> heroCountMap = new Dictionary<int, int>();

        // 영웅 ID를 카운트하여 중복 처리
        foreach (int heroId in heroIds)
        {
            if (heroCountMap.ContainsKey(heroId))
            {
                heroCountMap[heroId]++;
            }
            else
            {
                heroCountMap[heroId] = 1;
            }
        }

        List<SummonResultData> summonResults = new List<SummonResultData>();

        // SummonResultData 리스트 생성
        foreach (var entry in heroCountMap)
        {
            summonResults.Add(new SummonResultData
            {
                id = entry.Key,
                portraitPath = $"HeroImages/{entry.Key}",
                count = entry.Value // 획득한 개수 설정
            });
        }

        summonResultManager.UpdateSummonResults(summonResults);
    }

    private void UpdateHeroCollection(int[] heroIds)
    {
        heroCollectionManager.UpdateHeroCollection(heroIds);
    }

    [System.Serializable]
    public class GachaResult
    {
        public int[] result; // 가챠 결과로 얻은 영웅 ID 배열
    }
}
