using UnityEngine;
using System.Linq;
using EnumTypes;
using EventLibrary;

public class FormationManager : MonoBehaviour
{
    public GameObject changeLeaderModePanel; // 리더 카메라 변경 모드 패널
    public HeroController leader;
    public FollowerController[] followers;
    private bool isChangeLeaderMode;

    private void Awake()
    {
        // 씬에 있는 모든 HeroController를 찾음
        HeroController[] heroControllers = FindObjectsOfType<HeroController>();

        // 리더를 찾아 leader 변수에 할당
        leader = heroControllers.FirstOrDefault(hero => hero.IsLeader);

        // 리더가 아닌 나머지 HeroController를 followers 배열에 할당
        followers = heroControllers.Where(hero => !hero.IsLeader)
                                   .Select(hero => hero.GetComponent<FollowerController>())
                                   .Where(follower => follower != null)
                                   .ToArray();

        // 리더와 팔로워가 제대로 할당되었는지 확인
        if (leader == null)
        {
            Debug.LogError("Leader not found!");
        }
        else
        {
            SetLeader(leader.gameObject);
            Debug.Log("Leader assigned: " + leader.name);
        }

        if (followers.Length == 0)
        {
            Debug.LogError("Followers not found!");
        }
        else
        {
            foreach (var follower in followers)
            {
                SetFollower(follower.gameObject);
            }
            Debug.Log("Followers assigned: " + string.Join(", ", followers.Select(f => f.name)));
        }

        EventManager<FormationEvents>.StartListening(FormationEvents.OnChangeLeaderMode, EnableChangeLeaderMode);
        EventManager<FormationEvents>.StartListening(FormationEvents.SetLeader, OnSetLeader);
    }

    private void OnDestroy()
    {
        EventManager<FormationEvents>.StopListening(FormationEvents.OnChangeLeaderMode, EnableChangeLeaderMode);
        EventManager<FormationEvents>.StopListening(FormationEvents.SetLeader, OnSetLeader);
    }

    private void EnableChangeLeaderMode()
    {
        isChangeLeaderMode = true;
    }

    private void DisableChangeLeaderMode()
    {
        isChangeLeaderMode = false;
    }

    private void OnSetLeader()
    {
        DisableChangeLeaderMode();
    }

    public void SetLeader(GameObject newLeader)
    {
        if (!isChangeLeaderMode) return;

        var newLeaderController = newLeader.GetComponent<HeroController>();
        if (newLeaderController == null) return;

        var currentLeaderFollowerController = leader.GetComponent<FollowerController>();
        var newLeaderFollowerController = newLeader.GetComponent<FollowerController>();

        // 현재 리더를 팔로워로 설정
        leader.IsLeader = false;
        leader.enabled = false;
        currentLeaderFollowerController.enabled = true;

        // 새로운 리더를 설정
        newLeaderController.IsLeader = true;
        newLeaderController.enabled = true;
        newLeaderFollowerController.enabled = false;

        // 새로운 리더를 업데이트
        leader = newLeaderController;

        Debug.Log("New Leader assigned: " + leader.name);
    }

    public void SetFollower(GameObject follower)
    {
        var heroController = follower.GetComponent<HeroController>();
        var followerController = follower.GetComponent<FollowerController>();

        if (heroController != null) heroController.enabled = false;
        if (followerController != null) followerController.enabled = true;
    }
}
