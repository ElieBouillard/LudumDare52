using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraMainMenuManager : Singleton<CameraMainMenuManager>
{
    [SerializeField] private Transform _camMenuPos;
    [SerializeField] private Transform _camLobbyPos;
    [SerializeField] private float _transitionTime;
    
    public void EnableLobbyCameraPos(bool value)
    {
        Camera.main.transform.DOKill();
        Camera.main.transform.DOMove(value ? _camLobbyPos.position : _camMenuPos.position, _transitionTime).SetEase(Ease.Linear);
        Camera.main.transform.DORotate(value ? _camLobbyPos.eulerAngles : _camMenuPos.eulerAngles, _transitionTime).SetEase(Ease.Linear);
    }
}
