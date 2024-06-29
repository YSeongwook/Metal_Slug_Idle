using System;
using Gpm.Ui;

[Serializable]
public class HeroData : InfiniteScrollData
{
    public int id; // 캐릭터 id
    public string name; // 캐릭터 이름
    public string rank; // 캐릭터 등급 (D, C, B, A, S)
    public int level; // 캐릭터 레벨
    public int health; // 체력
    public int attack; // 공격력
    public string portraitPath; // 초상화 경로
    public int starLevel; // 강화 단계 (1~4성)
    public string type; // 캐릭터 타입 (공격형, 방어형, 지원형, 만능형)
}