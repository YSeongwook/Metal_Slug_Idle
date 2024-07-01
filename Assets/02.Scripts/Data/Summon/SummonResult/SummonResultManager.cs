using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class SummonResultManager : MonoBehaviour
{
    public InfiniteScroll summonResultScroll; // InfiniteScroll 컴포넌트 참조
    public Vector2 padding; // 패딩 값
    public Vector2 space; // 스페이스 값

    public void UpdateSummonResults(List<SummonResultData> newResults)
    {
        summonResultScroll.ClearData();
        foreach (var result in newResults)
        {
            summonResultScroll.InsertData(result, true);
            SetPaddingAndSpace();
        }
    }
    
    // 패딩과 스페이스 설정 메서드
    private void SetPaddingAndSpace()
    {
        summonResultScroll.SetPadding(padding);
        summonResultScroll.SetSpace(space);
    }
}