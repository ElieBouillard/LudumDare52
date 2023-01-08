using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WaitPlayerPanel : MonoBehaviour
{
    [SerializeField] private Image _waitPlayerImage0;
    [SerializeField] private Image _waitPlayerImage1;
    [SerializeField] private float _speedAnimation;

    private void Awake()
    {
        _waitPlayerImage0.fillAmount = 0;
        _waitPlayerImage1.fillAmount = 0;
    }
    
    
    private bool _isClockWise;
    private void Update()
    {
        if (!_isClockWise)
        {
            _waitPlayerImage0.fillAmount += _speedAnimation * Time.deltaTime;
            _waitPlayerImage1.fillAmount += _speedAnimation * Time.deltaTime;
            if (_waitPlayerImage0.fillAmount >= 1)
            {
                _isClockWise = true;
                _waitPlayerImage0.fillClockwise = _isClockWise;
                _waitPlayerImage1.fillClockwise = _isClockWise;
            }
                
        }
        else
        {
            _waitPlayerImage0.fillAmount -= _speedAnimation * Time.deltaTime;
            _waitPlayerImage1.fillAmount -= _speedAnimation * Time.deltaTime;
        
            if (_waitPlayerImage0.fillAmount <= 0)
            {
                _isClockWise = false;
                _waitPlayerImage0.fillClockwise = _isClockWise;
                _waitPlayerImage1.fillClockwise = _isClockWise;
            }
        }
    }
}
