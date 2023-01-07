using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;
using static CMF.AdvancedWalkerController;

public class PlayerLocalAnimations : MonoBehaviour
{
    [SerializeField] private float _smooth;
    [SerializeField] private AdvancedWalkerController _controller;
    private Animator _animator;
    
    private ControllerState _lastControllerState = ControllerState.Falling;
    
    private static readonly int VelocityXParameter = Animator.StringToHash("VelocityX");
    private static readonly int VelocityYParameter = Animator.StringToHash("VelocityY");
    private static readonly int JumpAnimParameter = Animator.StringToHash("Jump");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_controller.CurrentControllerState == ControllerState.Grounded)
        {
            Vector2 velocity = new Vector2(Vector3.Dot(_controller.GetVelocity() / _controller.runSpeed, transform.right), 
                Vector3.Dot(_controller.GetVelocity() / _controller.runSpeed, transform.forward));

            _animator.SetFloat(VelocityXParameter,
                Mathf.Lerp(_animator.GetFloat(VelocityXParameter), velocity.x, _smooth * Time.deltaTime));
            _animator.SetFloat(VelocityYParameter,
                Mathf.Lerp(_animator.GetFloat(VelocityYParameter), velocity.y, _smooth * Time.deltaTime));

            if (_lastControllerState != ControllerState.Grounded)
            {
                _animator.SetBool(JumpAnimParameter, false);
                _lastControllerState = ControllerState.Grounded;
            }
        }
        else if(_controller.CurrentControllerState == ControllerState.Jumping)
        {
            if (_lastControllerState != ControllerState.Jumping)
            {
                _animator.SetBool(JumpAnimParameter, true);
                _lastControllerState = ControllerState.Jumping;
            }
        }
        else if(_controller.CurrentControllerState == ControllerState.Falling)
        {
            if (_lastControllerState != ControllerState.Falling)
            {
                _animator.SetBool(JumpAnimParameter, true);
                _lastControllerState = ControllerState.Falling;
            }
        }
    }
}