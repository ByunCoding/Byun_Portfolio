using UnityEngine;

public class GolemsGethitState : IState<Golems>
{
    protected Golems _owner;

    public GolemsGethitState(Golems owner)
    {
        _owner = owner;
    }

    public void OperateEnter(Golems sender)
    {
        sender._navAgent.isStopped = true;
        _owner._animator.SetTrigger("GetHit");
        _owner._isAnimPlaying = true;
    }

    public void OperateExit(Golems sender)
    {
        _owner._animator.ResetTrigger("GetHit");
        sender._navAgent.isStopped = false;
        _owner._isAnimPlaying = false;
    }

    public void OperateUpdate(Golems sender)
    {
        if (_owner._isAnimPlaying) return;

        if (_owner.DetectingTarget())
        {
            if (_owner.checkAttackRange())
            {
                _owner.ChangeAttackState();
            }
            else
            {
                _owner.ChangeChaseState();
            }
        }
        else
        {
            _owner.ChangePatrolState();
        }
    }
}
