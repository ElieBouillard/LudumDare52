using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDistantAnimations : MonoBehaviour
{
    [SerializeField] private float _smooth;
    
    private Animator _animator;
    private PlayerDistantHealth _health;
    
    public bool LastJump;
    private static readonly int VelocityXParameter = Animator.StringToHash("VelocityX");
    private static readonly int VelocityYParameter = Animator.StringToHash("VelocityY");
    private static readonly int JumpParameter = Animator.StringToHash("Jump");
    private static readonly int Die = Animator.StringToHash("Die");
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _health = GetComponentInParent<PlayerDistantHealth>();
    }

    public void SetAnim(Vector2 velocity, bool jump)
    {
        TargetVelocity = velocity;

        if (jump != LastJump)
        {
            _animator.SetBool(JumpParameter,jump);
            LastJump = jump;
        }
    }

    public Vector2 TargetVelocity;


    private void Update()
    {
        _animator.SetFloat(VelocityXParameter, Mathf.Lerp(_animator.GetFloat(VelocityXParameter), TargetVelocity.x, _smooth * Time.deltaTime)); 
        _animator.SetFloat(VelocityYParameter, Mathf.Lerp(_animator.GetFloat(VelocityYParameter),  TargetVelocity.y,_smooth * Time.deltaTime));
    }

    public void PlayDeathAnim()
    {
        _animator.SetBool(Die, true);
        StartCoroutine(StopDeathAnim());
    }

    private IEnumerator StopDeathAnim()
    {
        yield return new WaitForSeconds(5f);
        _animator.SetBool(Die, false);
        
        _health.InitializeHealth();
    }
}