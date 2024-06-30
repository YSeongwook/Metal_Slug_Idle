using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using EnumTypes;
using EventLibrary;
using UnityEngine.Networking;

public class GachaManager : MonoBehaviour
{
    private string gachaUrl = "https://us-central1-unifire-ebcc1.cloudfunctions.net/gacha"; // 가챠 API의 URL

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
        }
        else
        {
            Debug.LogError("Gacha request failed: " + request.error); // 에러 처리
        }
    }

    private void UpdateUIWithGachaResult(int[] heroIds)
    {
        // 결과를 UI에 업데이트하는 로직을 구현
    }

    [System.Serializable]
    public class GachaResult
    {
        public int[] result; // 가챠 결과로 얻은 영웅 ID 배열
    }
}
