using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _fireAudioSource;
    [SerializeField] private AudioSource _hitMarkerSource;
    [SerializeField] private AudioSource _hitSource;
    [SerializeField] private AudioSource _dieSource;
    [SerializeField] private AudioSource _reloadSource;
    
    [Header("MovementSound")]
    [SerializeField] private AudioSource _movementAudioSource;
    [SerializeField] private AudioClip[] _walkAndRunSounds;
    [SerializeField] private AdvancedWalkerController _controller;
    [SerializeField] private PlayerDistantAnimations _distantAnimations;

    public void PlayFireSound()
    {
        _fireAudioSource.Play();
    }

    public void PlayHitMarkerSound()
    {
        _hitMarkerSource.Play();
    }

    public void PlayHit()
    {
        _hitSource.Play();
    }

    public void PlayDieSound()
    {
        _dieSource.Play();
    }

    public void PlayReloadSound()
    {
        _reloadSource.Play();
    }
    
    private bool _isMovementPlaying;
    private bool _isWalking;

    private bool _isGrounded;
    private float _velocity;
    private void Update()
    {
        if (_controller != null)
        {
            _velocity = _controller.GetVelocity().magnitude / _controller.runSpeed;
            _isGrounded = _controller.CurrentControllerState == AdvancedWalkerController.ControllerState.Grounded;
        }

        if (_distantAnimations != null)
        {
            _velocity = _distantAnimations.TargetVelocity.magnitude;
            _isGrounded = !_distantAnimations.LastJump;
        }
        
        if(_isGrounded)
        {
            if (_velocity > 0)
            {
                if (_velocity <= 0.501)
                {
                    if (!_isWalking)
                    {
                        _movementAudioSource.Stop();
                        _movementAudioSource.clip =  _walkAndRunSounds[0];
                        _isMovementPlaying = false;
                        _isWalking = true;
                    }
                }
                else
                {
                    if (_isWalking)
                    {
                        _movementAudioSource.Stop();
                        _movementAudioSource.clip =  _walkAndRunSounds[1];
                        _isMovementPlaying = false;
                        _isWalking = false;
                    }
                }

                if (!_isMovementPlaying)
                {
                    _movementAudioSource.Play();
                    _isMovementPlaying = true;
                }
            }
            else
            {
                if (_isMovementPlaying)
                {
                    _movementAudioSource.Stop();
                    _isMovementPlaying = false;
                }
            }
        }
        else
        {
            if (_isMovementPlaying)
            {
                _movementAudioSource.Stop();
                _isMovementPlaying = false;
            }
        }
    }
}
