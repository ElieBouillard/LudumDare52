using System.Collections;
using System.Collections.Generic;
using System.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : Singleton<StatisticsPanel>
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _oxygenBar;
    [SerializeField] private Image _foodBar;
    [SerializeField] private Image _waterBar;

    public void SetBarValue(float healthAmount, float oxygenAmount)
    {
        _healthBar.fillAmount = healthAmount;
        _oxygenBar.fillAmount = oxygenAmount;
    }
}