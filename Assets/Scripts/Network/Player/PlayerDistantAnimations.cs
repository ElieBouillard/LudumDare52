using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDistantAnimations : MonoBehaviour
{
    [SerializeField] private float _smooth;
    
    private Animator _animator;
    
    private bool _lastJump;
    private static readonly int VelocityXParameter = Animator.StringToHash("VelocityX");
    private static readonly int VelocityYParameter = Animator.StringToHash("VelocityY");
    private static readonly int JumpParameter = Animator.StringToHash("Jump");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAnim(Vector2 velocity, bool jump)
    {
        _targetVelocity = velocity;

        if (jump != _lastJump)
        {
            _animator.SetBool(JumpParameter,jump);
            _lastJump = jump;
        }
    }

    private Vector2 _targetVelocity;
    
    private void Update()
    {
        _animator.SetFloat(VelocityXParameter, Mathf.Lerp(_animator.GetFloat(VelocityXParameter), _targetVelocity.x, _smooth * Time.deltaTime)); 
        _animator.SetFloat(VelocityYParameter, Mathf.Lerp(_animator.GetFloat(VelocityYParameter),  _targetVelocity.y,_smooth * Time.deltaTime));
    }
}