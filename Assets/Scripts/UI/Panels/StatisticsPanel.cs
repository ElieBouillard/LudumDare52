using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _oxygenBar;
    [SerializeField] private Image _foodBar;
    [SerializeField] private Image _waterBar;

    public void SetBarValue(BarType type, float amount)
    {
        switch (type)
        {
            case BarType.Health:
                _healthBar.DOFillAmount(amount, 1f);
                break;
            case BarType.Oxygen:
                _oxygenBar.DOFillAmount(amount, 1f);
                break;
            case BarType.Food:
                _foodBar.DOFillAmount(amount, 1f);
                break;
            case BarType.Water:
                _waterBar.DOFillAmount(amount, 1f);
                break;
        }
    }
}

public enum BarType
{
    Health,
    Oxygen,
    Food,
    Water
}
