using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GolemsPatrolState : IState<Golems>
{
    protected Golems _owner;

    Vector3 _destinationPos;    // 패트롤시 목적지 
    private float _spendTime = 0.0f;
    private float _lapTime = 0.5f;

    public GolemsPatrolState(Golems owner)
    {
        _owner = owner;
    }

    public void OperateEnter(Golems sender)
    {
        _destinationPos = _owner.GetPatrolPositionOnNavMesh();
        sender._animator.SetBool("Walk", true);
        _owner.PatrolMove(_destinationPos);
    }

    public void OperateExit(Golems sender)
    {

    }

    public void OperateUpdate(Golems sender)
    {
        if (_owner.DetectingTarget() && _owner.checkChaseRange())
        {
            _owner.ChangeChaseState();
        }
        else
        {
            if (_owner.IsArrived())
            {
                _spendTime += Time.deltaTime;

                if (_spendTime >= _lapTime)
                {
                    _spendTime = 0.0f;
                    _destinationPos = _owner.GetPatrolPositionOnNavMesh();
                    _owner.PatrolMove(_destinationPos);
                }

            }
        }
    }
}
