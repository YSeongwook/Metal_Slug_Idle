using UnityEngine;

// 0리더 캐릭터의 동작을 정의하는 클래스
public class LeaderController : HeroController
{
    private float _lastUserInputTime;
    public float autoMoveDelay = 0.5f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            IsUserControlled = true;
            _isMovingToTarget = false;
            _lastUserInputTime = Time.time;
            TransitionToState(MoveState);
        }
        else if (Time.time - _lastUserInputTime > autoMoveDelay)
        {
            if (!IsMoving)
            {
                IsUserControlled = false;
                TransitionToState(IdleState);
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!IsUserControlled && !IsMoving)
        {
            TransitionToState(IdleState);
        }
    }
}