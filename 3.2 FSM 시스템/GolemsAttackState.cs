using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GolemsAttackState : IState<Golems>
{
    protected Golems _owner;

    private float attackAngleThreshold = 30f;
    private float _rotationSpeed = 5.0f;

    public GolemsAttackState(Golems owner)
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
        if (_owner.checkAttackRange() && _owner._playerTr != null)
        {
            Vector3 toPlayer = _owner._playerTr.position - _owner.transform.position;
            toPlayer.y = 0.0f;

            float angle = Vector3.Angle(_owner.transform.forward, toPlayer);

            if (angle <= attackAngleThreshold)
            {
                if (!_owner.IsAttack())
                {
                    _owner.SetStartAttack();
                    _owner._animator.SetTrigger("Attack1");

                    if (_owner.checkAttackRange())
                    {
                        if (!_owner.IsAttack())
                        {
                            _owner.SetStartAttack();
                            _owner._animator.SetTrigger("Attack2");
                            return;
                        }
                    }
                    else
                    {
                        _owner.ChangeChaseState();
                    }
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(toPlayer);
                _owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (_owner.checkChaseRange())
            {
                _owner.ChangeChaseState();
            }
        }

    }

}

