using UnityEngine;

public class GolemsChaseState : IState<Golems>
{
    protected Golems _owner;

    public GolemsChaseState(Golems owner)
    {
        _owner = owner;
    }

    public void OperateEnter(Golems sender)
    {

    }

    public void OperateExit(Golems sender)
    {

    }

    public void OperateUpdate(Golems sender)
    {
        if (_owner.checkChaseRange())
        {
            if (_owner.checkAttackRange())
            {
                _owner.ChangeAttackState();
            }
            else
            {
                _owner.MoveChase();
            }
        }
        else
        {
            _owner.ChangePatrolState();
        }

        //if (sender._hasBeenAttacked)
        //{
        //    sender._animator.SetBool("Run", true);
        //    sender._navAgent.speed = _owner._moveSpeed * 2.0f;
        //}
        //else
        //{
        //    sender._animator.SetBool("Walk", true);
        //    sender._navAgent.speed = _owner._moveSpeed;
        //}

    }
}
