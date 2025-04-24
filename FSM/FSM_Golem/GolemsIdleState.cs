using Unity.VisualScripting;
using UnityEngine;

public class GolemsIdleState : IState<Golems>
{
    protected Golems _owner;

    //private float _waitingTime = 2.0f;
    //private float _spendTime = 0.0f;


    public GolemsIdleState(Golems owner)
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
        if (_owner.DetectingTarget())
        {
            _owner.ChangeChaseState();
        }
        else
        {
            _owner.ChangePatrolState();
        }
    }
}
//_spendTime += Time.deltaTime;

//if(_spendTime >= _waitingTime)
//{
//}