using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class DataManager<TData> : MonoBehaviour where TData : InfiniteScrollData
{
    public InfiniteScroll infiniteScroll; // InfiniteScroll 컴포넌트 참조
    public Vector2 padding; // 패딩 값
    public Vector2 space; // 스페이스 값
    public string fileName; // JSON 파일 이름

    protected virtual void Start()
    {
        LoadData(); // 데이터 로드
    }

    // 데이터를 로드하고 스크롤 뷰에 추가하는 메서드
    public void LoadData()
    {
        List<TData> data = DataLoader<TData>.LoadDataFromJson(fileName);

        infiniteScroll.ClearData(); // 데이터 초기화
        infiniteScroll.InsertData(data.ToArray(), true); // 데이터 추가
        SetPaddingAndSpace(); // 패딩과 스페이스 설정
        infiniteScroll.MoveToFirstData(); // 첫 번째 데이터로 이동
    }

    // 패딩과 스페이스 설정 메서드
    private void SetPaddingAndSpace()
    {
        infiniteScroll.SetPadding(padding);
        infiniteScroll.SetSpace(space);
    }
}