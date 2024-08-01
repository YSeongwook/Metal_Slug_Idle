using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class FormationManager : Singleton<FormationManager>
{
    public GameObject changeLeaderModePanel; // 리더 카메라 변경 모드 패널
    public HeroController leader; // 인스펙터에서 할당된 리더
    public CameraFollow cameraFollow;
    public List<FollowerController> followers;

    private FollowerController _leaderFollowerController;
    private bool _isChangeLeaderMode;

    protected override void Awake()
    {
        base.Awake();
        
        // followers 리스트 초기화
        followers = new List<FollowerController>();

        // 씬에 있는 모든 HeroController를 찾음
        HeroController[] heroControllers = FindObjectsOfType<HeroController>();

        // 인스펙터에서 할당된 리더가 없는 경우 첫 번째 영웅을 리더로 설정
        if (leader == null && heroControllers.Length > 0)
        {
            leader = heroControllers[0];
            leader.IsLeader = true;
        }

        // 리더 설정
        if (leader != null)
        {
            leader.IsLeader = true;
            _leaderFollowerController = leader.GetComponent<FollowerController>();
            if (_leaderFollowerController != null)
            {
                _leaderFollowerController.leader = leader;
                _leaderFollowerController.enabled = false;
            }
        }

        // 리더가 아닌 나머지 HeroController를 followers 리스트에 할당
        foreach (var hero in heroControllers)
        {
            if (hero == leader) continue;
            
            var follower = hero.GetComponent<FollowerController>();
            if (follower != null && !followers.Contains(follower))
            {
                followers.Add(follower);
            }
                
            if (follower.gameObject.GetComponent<HeroController>().IsLeader)
            {
                followers.Remove(follower);
            }
        }
    }

    private void Start()
    {
        // 리더와 팔로워가 제대로 할당되었는지 확인
        if (leader == null)
        {
            DebugLogger.LogError("Leader not found!");
        }

        if (followers.Count == 0)
        {
            DebugLogger.LogError("Followers not found!");
        }
        else
        {
            foreach (var follower in followers)
            {
                SetFollower(follower.gameObject);
            }
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
        _isChangeLeaderMode = true;
        changeLeaderModePanel.SetActive(true);
    }

    private void DisableChangeLeaderMode()
    {
        _isChangeLeaderMode = false;
        changeLeaderModePanel.SetActive(false);
    }

    private void OnSetLeader()
    {
        DisableChangeLeaderMode();
    }

    public void SetLeader(GameObject newLeader)
    {
        if (!_isChangeLeaderMode) return;

        var newLeaderController = newLeader.GetComponent<HeroController>();
        if (newLeaderController == null || newLeaderController == leader) return;

        var currentLeaderFollowerController = leader.GetComponent<FollowerController>();
        var newLeaderFollowerController = newLeader.GetComponent<FollowerController>();

        // 현재 리더를 팔로워로 설정
        leader.IsLeader = false;
        if (currentLeaderFollowerController != null)
        {
            leader.enabled = false;
            currentLeaderFollowerController.enabled = true;
            currentLeaderFollowerController.InitializeFollower(); // 초기화 메서드 호출
            if (!followers.Contains(currentLeaderFollowerController))
            {
                followers.Add(currentLeaderFollowerController);
            }
        }

        // 새로운 리더를 설정
        newLeaderController.IsLeader = true;
        if (newLeaderFollowerController != null)
        {
            newLeaderFollowerController.enabled = false;
            followers.Remove(newLeaderFollowerController);
            newLeaderController.enabled = true;
        }

        // 새로운 리더 업데이트
        leader = newLeaderController;
        leader.LoadHeroStats();
        _leaderFollowerController = leader.transform.GetComponent<FollowerController>();
        leader.transform.GetComponent<FollowerController>().leader = leader;

        // 카메라 추적 대상 새로운 리더로 업데이트
        cameraFollow.leader = leader.transform;

        // 팔로워들 업데이트
        foreach (var follower in followers)
        {
            follower.leader = leader;
            // follower.InitializeFollower();
            follower.LoadHeroStats();
        }
        
        UpdateFormationOffSets();

        DebugLogger.Log("New Leader assigned: " + leader.name);
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

    public void UpdateFormationOffSets()
    {
        var leaderOffset = _leaderFollowerController.formationOffset;
        foreach (var follower in followers)
        {
            follower.formationOffset = RoundToOneDecimalPlace(follower.formationOffset - leaderOffset);
        }

        _leaderFollowerController.formationOffset = Vector3.zero;
    }
    
    private Vector3 RoundToOneDecimalPlace(Vector3 vector)
    {
        return new Vector3(
            Mathf.Round(vector.x * 10f) / 10f,
            Mathf.Round(vector.y * 10f) / 10f,
            Mathf.Round(vector.z * 10f) / 10f
        );
    }
}
