using System.ComponentModel;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    private Animator _animator;
    public Transform _leftHand = null;
    //public Transform _rightHand = null;

    private int layerIndex_Weapons;

    public bool _ikActive = false;

    private void OnAnimatorIK(int layerIndex)
    {
        if(layerIndex != layerIndex_Weapons)
        {
            return;
        }

        if (_animator)
        {
            if (_ikActive)
            {
                if(_leftHand != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHand.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHand.rotation);
                }
                //if (_rightHand != null)
                //{
                //    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                //    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

                //    _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHand.position);
                //    _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHand.rotation);
                //}

            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetLookAtWeight(0);

            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        layerIndex_Weapons = _animator.GetLayerIndex("Rifle");

        Rifle rifle = GetComponent<Rifle>();
        if(rifle != null)
        {
            _leftHand = rifle.GetLeftHand();
            //_rightHand = rifle.GetRightHand();
        }

    }

    public void SetHandPos(Transform leftHand, Transform rightHand = null)
    {
        _leftHand = leftHand;
        //_rightHand = rightHand;
    }
}