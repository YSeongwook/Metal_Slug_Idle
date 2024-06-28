using System;
using Gpm.Ui;

[Serializable]
public class SummonData : InfiniteScrollData
{
    public string name;        // 배너 이름
    public string explain;     // 배너 설명
    public string backgroundPath; // 배경 스프라이트 경로
    public string bannerPath; // 배너 스프라이트 경로
    public int endTime;      // 픽업 종료 시간
}