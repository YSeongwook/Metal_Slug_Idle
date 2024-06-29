using System.Collections;
using EnumTypes;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using EventLibrary;

public class GachaManager : MonoBehaviour
{
    // Firebase Functions URL
    private string gachaUrl = "https://us-central1-unifire-ebcc1.cloudfunctions.net/gacha";

    // 가챠 버튼 클릭 시 호출되는 메서드
    public void PerformGacha()
    {
        // 현재 사용자 ID를 가져옴
        string userId = AuthManager.Instance.GetCurrentUser().UserId;
        // 가챠 결과를 받아오기 위한 코루틴 시작
        StartCoroutine(GachaCoroutine(userId));
    }

    // 가챠 결과를 받아오는 코루틴
    private IEnumerator GachaCoroutine(string userId)
    {
        // 사용자 ID를 포함한 JSON 데이터 생성
        var json = JsonConvert.SerializeObject(new { userId = userId });
        // HTTP POST 요청 생성
        var request = new UnityWebRequest(gachaUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송 및 응답 대기
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 응답 데이터를 JSON으로 파싱하여 가챠 결과를 얻음
            var result = JsonConvert.DeserializeObject<GachaResult>(request.downloadHandler.text);
            Debug.Log("Gacha result: " + result.result);

            // 가챠 결과를 로컬 저장소에 저장
            SaveHeroToLocal(result.result);

            // 가챠 결과를 UI에 업데이트
            UpdateUIWithGachaResult(result.result);
        }
        else
        {
            // 요청 실패 시 오류 로그 출력
            Debug.LogError("Gacha request failed: " + request.error);
        }
    }

    // 가챠 결과를 로컬 파일에 저장하는 메서드
    private void SaveHeroToLocal(int heroId)
    {
        // 로컬 파일 경로 설정
        string path = Application.persistentDataPath + "/HeroCollection.json";
        HeroCollection heroCollection;

        // 파일이 존재하면 기존 데이터를 불러오고, 그렇지 않으면 새로운 데이터 생성
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            heroCollection = JsonConvert.DeserializeObject<HeroCollection>(json);
        }
        else
        {
            heroCollection = new HeroCollection();
        }

        // 가챠 결과를 히어로 컬렉션에 추가
        bool heroFound = false;
        foreach (var item in heroCollection.heroes)
        {
            if (item.id == heroId)
            {
                item.owned = true;
                heroFound = true;
                break;
            }
        }

        // 해당 영웅이 히어로 컬렉션에 없으면 새로 추가
        if (!heroFound)
        {
            heroCollection.heroes.Add(new HeroCollectionItem { id = heroId, owned = true });
        }

        // 업데이트된 데이터를 JSON으로 직렬화하여 파일에 저장
        string newJson = JsonConvert.SerializeObject(heroCollection, Formatting.Indented);
        System.IO.File.WriteAllText(path, newJson);

        // 이벤트 발생: HeroCollection이 업데이트되었음을 알림
        EventManager<DataEvents>.TriggerEvent(DataEvents.HeroCollectionUpdated, heroCollection);
    }

    // 가챠 결과를 UI에 업데이트하는 메서드
    private void UpdateUIWithGachaResult(int heroId)
    {
        // 가챠 결과를 UI에 반영하는 로직을 추가합니다.
        // 예: 새로운 영웅을 화면에 표시
    }

    // 가챠 결과를 저장하기 위한 클래스
    [System.Serializable]
    public class GachaResult
    {
        public int result;
    }
}
