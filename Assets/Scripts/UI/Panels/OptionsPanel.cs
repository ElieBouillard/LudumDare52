using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : Panel
{
    [SerializeField] private bool _isInGame;
    
    [Header("References")]
    [SerializeField] private Button _returnButton;

    protected override void AssignButtonsReference()
    {
        _returnButton.onClick.AddListener(OnCLickReturn);
    }

    protected void OnCLickReturn()
    {
        PanelManager.Instance.EnablePanel(_isInGame ? PanelType.Pause : PanelType.MainMenu);
    }
}
