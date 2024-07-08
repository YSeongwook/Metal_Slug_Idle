using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using EnumTypes;
using EventLibrary;

public class FormationManager : MonoBehaviour
{
    public GameObject changeLeaderModePanel; // 리더 카메라 변경 모드 패널
    public HeroController leader;
    public List<FollowerController> followers;
    private bool isChangeLeaderMode;

    private void Awake()
    {
        // followers 리스트 초기화
        followers = new List<FollowerController>();

        // 씬에 있는 모든 HeroController를 찾음
        HeroController[] heroControllers = FindObjectsOfType<HeroController>();

        if (leader == null)
        {
            // 리더를 찾아 leader 변수에 할당
            leader = heroControllers.FirstOrDefault(hero => hero.IsLeader);
        }

        if (leader != null)
        {
            leader.IsLeader = true;
            leader.GetComponent<FollowerController>().enabled = false;
        }

        // 리더가 할당되지 않은 경우, 첫 번째 영웅을 리더로 설정
        if (leader == null && heroControllers.Length > 0)
        {
            leader = heroControllers[4];
            leader.IsLeader = true;
        }

        // 리더가 아닌 나머지 HeroController를 followers 리스트에 할당
        foreach (var hero in heroControllers)
        {
            if (!hero.IsLeader)
            {
                var follower = hero.GetComponent<FollowerController>();
                if (follower != null && !followers.Contains(follower))
                {
                    followers.Add(follower);

                    if (follower.gameObject.GetComponent<HeroController>().IsLeader)
                    {
                        followers.Remove(follower);
                    }
                }
            }
        }
    }

    private void Start()
    {
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

        if (followers.Count == 0)
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
        changeLeaderModePanel.SetActive(true);
    }

    private void DisableChangeLeaderMode()
    {
        isChangeLeaderMode = false;
        changeLeaderModePanel.SetActive(false);
    }

    private void OnSetLeader()
    {
        DisableChangeLeaderMode();
    }

    public void SetLeader(GameObject newLeader)
    {
        if (!isChangeLeaderMode) return;
        DebugLogger.Log("SetLeader");

        var newLeaderController = newLeader.GetComponent<HeroController>();
        if (newLeaderController == null) return;

        var currentLeaderFollowerController = leader.GetComponent<FollowerController>();
        var newLeaderFollowerController = newLeader.GetComponent<FollowerController>();

        // 현재 리더를 팔로워로 설정
        leader.IsLeader = false;
        leader.enabled = false;
        if (currentLeaderFollowerController != null)
        {
            currentLeaderFollowerController.enabled = true;
            currentLeaderFollowerController.InitializeFollower(); // 초기화 메서드 호출
            followers.Add(currentLeaderFollowerController);
        }

        // 새로운 리더를 설정
        newLeaderController.IsLeader = true;
        newLeaderController.enabled = true;
        if (newLeaderFollowerController != null)
        {
            newLeaderFollowerController.enabled = false;
            followers.Remove(newLeaderFollowerController);
        }

        // 새로운 리더를 업데이트
        leader = newLeaderController;

        // 팔로워들의 리더를 업데이트
        foreach (var follower in followers)
        {
            follower.leader = leader;
        }

        Debug.Log("New Leader assigned: " + leader.name);
    }

    private void SetFollower(GameObject follower)
    {
        var heroController = follower.GetComponent<HeroController>();
        var followerController = follower.GetComponent<FollowerController>();

        if (heroController != null) heroController.enabled = false;
        if (followerController != null)
        {
            followerController.enabled = true;
            followerController.InitializeFollower(); // 초기화 메서드 호출
        }

        // 팔로워의 리더를 설정
        followerController.leader = leader;
    }
}
