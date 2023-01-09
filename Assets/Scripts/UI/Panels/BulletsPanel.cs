using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletsPanel : Singleton<BulletsPanel>
{
    [SerializeField] private TMP_Text _bulletsText;
    [SerializeField] private Image _reloadImage;
    [SerializeField] private GameObject _hitMarker;
    
    public void UpdateBulletsAmount(int amount)
    {
        _bulletsText.text = $"{amount}/6";
    }

    public void StartReloadImage(float value)
    {
        _reloadImage.fillAmount = 0;
        _reloadImage.DOFillAmount(1, value).SetEase(Ease.Linear).OnComplete(()=> _reloadImage.fillAmount = 0f);
    }

    public void PlayHitMarker()
    {
        _hitMarker.SetActive(true);
        StartCoroutine(StopHitMarket());
    }

    private IEnumerator StopHitMarket()
    {
        yield return new WaitForSeconds(0.15f);

        _hitMarker.SetActive(false);
    }
}
