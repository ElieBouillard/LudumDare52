using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : Panel
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _disconnectionButton;

    protected override void AssignButtonsReference()
    {
        _resumeButton.onClick.AddListener(ResumeGame);
        _optionsButton.onClick.AddListener(Options);
        _disconnectionButton.onClick.AddListener(Disconnect);
    }

    private void ResumeGame()
    {
        PanelManager.Instance.EnablePause(false);
    }

    private void Options()
    {
        PanelManager.Instance.EnablePanel(PanelType.Options);
    }
    
    private void Disconnect()
    {
        NetworkManager.Instance.Leave();
    }
}
