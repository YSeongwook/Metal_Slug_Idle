using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class SummonResultManager : MonoBehaviour
{
    public InfiniteScroll summonResultScroll; // InfiniteScroll 컴포넌트 참조

    public void UpdateSummonResults(List<SummonResultData> newResults)
    {
        summonResultScroll.ClearData();
        foreach (var result in newResults)
        {
            summonResultScroll.InsertData(result, true);
        }
    }
}