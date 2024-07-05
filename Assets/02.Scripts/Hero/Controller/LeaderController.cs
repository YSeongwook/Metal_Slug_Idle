using UnityEngine;

public class LeaderController : HeroController
{
    private void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            IsUserControlled = true;
            _isMovingToTarget = false;
            LastUserInputTime = Time.time;
            TransitionToState(MoveState);
        }
        else if (Time.time - LastUserInputTime > autoMoveDelay)
        {
            if (!IsMoving)
            {
                IsUserControlled = false;
                TransitionToState(IdleState);
                this.Animator.SetFloat(Speed, 0);
            }
        }
        else
        {
            this.Animator.SetFloat(Speed, 0);
        }
    }
}