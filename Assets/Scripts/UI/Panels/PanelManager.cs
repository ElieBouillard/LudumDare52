using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] private Panel[] _panels;
    [SerializeField] private Image _crosshair;

    [SerializeField] private GameObject _waitForPlayersPanel;
    [SerializeField] private CooldownPanel _cooldownPanel;
    
    private NetworkManager _networkManager;
    private bool _isPause;

    protected override void Awake()
    {
        base.Awake();

        EnablePanel(PanelType.MainMenu);
    }

    private void Start()
    {
        _networkManager = NetworkManager.Instance;
        
        if(_networkManager.GameState == GameState.OffLine || _networkManager.GameState == GameState.Lobby)
            EnableCursor(true);
        
        if(_networkManager.GameState == GameState.Gameplay)
            EnableCursor(false);
    }

    private void Update()
    {
        if (_networkManager.GameState == GameState.Gameplay)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EnablePause(!_isPause);
            }
        }
    }
    
    public void EnablePanel(PanelType panelType)
    {
        foreach (var panel in _panels)
        {
            panel.gameObject.SetActive(panel.PanelType == panelType);
        }
    }

    public void EnableCursor(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;

        if (NetworkManager.Instance.GameState == GameState.Gameplay)
            _crosshair.enabled = !value;
    }

    public void EnablePause(bool value)
    {
        EnablePanel(value ? PanelType.Pause : PanelType.MainMenu);
        _isPause = value;
        EnableCursor(value);

        if (!value) if (!_cooldownPanel.GameIsStarted) return;
        
        ((PlayerGameIdentity)NetworkManager.Instance.LocalPlayer).EnableInput(!value);
    }

    public void EnableGame()
    {
        _waitForPlayersPanel.SetActive(false);
        _cooldownPanel.gameObject.SetActive(true);
        _cooldownPanel.StartCouldown();
    }
}

public enum PanelType
{
    MainMenu = 1,
    Options,
    Lobby,
    Pause,
}