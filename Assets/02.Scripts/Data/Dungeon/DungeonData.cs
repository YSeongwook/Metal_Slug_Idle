using System;
using Gpm.Ui;

[Serializable]
public class DungeonData : InfiniteScrollData
{
    public string name;        // 던전 이름
    public string explain;     // 던전 설명
    public string backgroundPath; // 배경 스프라이트 경로
    public string keyIconPath; // 입장 열쇠 스프라이트 경로
    public int keyRemain;      // 남은 열쇠 수
}