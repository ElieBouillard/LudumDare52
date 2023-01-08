using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    [Header("InitialStatistics")]
    [SerializeField] private float _initialHealth;
    [SerializeField] private float _initialOxygen;
    [SerializeField] private float _initialFood;
    [SerializeField] private float _initialWater;

    [Space(10)] [Header("DecreaseValue")] 
    [SerializeField] private float _decreaseHealth;
    [SerializeField] private float _decreaseOxygen;
    [SerializeField] private float _decreaseFood;
    [SerializeField] private float _decreaseWater;

    private StatisticsPanel _statisticsPanel;
    
    private float _currHealth;
    private float _currOxygen;
    private float _currFood;
    private float _currWater;

    private bool _isInBase;
    
    private void Start()
    {
        _statisticsPanel = StatisticsPanel.Instance;
        
        SetupStatistics();
        UpdateInterface();
    }

    private void Update()
    {
        if (_isInBase) return;
        
        _currHealth -= _decreaseHealth * Time.deltaTime;
        _currOxygen -= _decreaseOxygen * Time.deltaTime;
        _currFood -= _decreaseFood * Time.deltaTime;
        _currWater -= _decreaseWater * Time.deltaTime;

        UpdateInterface();
    }

    public void SetupStatistics()
    {
        _currHealth = _initialHealth;
        _currOxygen = _initialOxygen;
        _currFood = _initialFood;
        _currWater = _initialWater;
    }
    
    private void UpdateInterface()
    {
        _statisticsPanel.SetBarValue(_currHealth / _initialHealth, _currOxygen / _initialOxygen, _currFood / _initialFood, _currWater/ _initialWater );
    }
}