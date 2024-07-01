using System;
using Gpm.Ui;

[Serializable]
public class SummonResultData : InfiniteScrollData
{
    public string portraitPath; // 배경 스프라이트 경로
    public int id; // 영웅 id
    public int count; // 소환된 개수
    public string rank; // 영웅 랭크
}