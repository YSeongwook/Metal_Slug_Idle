using System.Collections.Generic;

public class SummonResultManager : DataManager<SummonResultData>
{
    public override void LoadData()
    {
    }

    // 가챠 결과 데이터를 업데이트하는 메서드
    public void UpdateSummonResults(List<SummonResultData> newResults)
    {
        data = newResults;
        infiniteScroll.ClearData();
        infiniteScroll.InsertData(data.ToArray(), true);
        SetPaddingAndSpace();
        infiniteScroll.MoveToFirstData();
    }
}