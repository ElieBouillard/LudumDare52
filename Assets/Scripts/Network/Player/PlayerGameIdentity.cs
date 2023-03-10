using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerGameIdentity : PlayerIdentity
{
    [SerializeField] private TMP_Text _pseudoText;

    public MovementReceiver MovementReceiver { private set; get; }

    public PlayerDistantAnimations Animations { private set; get; }
    
    public PlayerStatistics LocalHealth { private set; get; }
    public PlayerDistantHealth DistantHealth { private set; get; }
    public PlayerAim Aim { private set; get; }

    private CharacterKeyboardInput _input;

    [SerializeField] private CameraMouseInput _cameraInput;
    
    private void Awake()
    {
        MovementReceiver = GetComponent<MovementReceiver>();
        Animations = GetComponentInChildren<PlayerDistantAnimations>();
        Aim = GetComponent<PlayerAim>();
        _input = GetComponent<CharacterKeyboardInput>();
        LocalHealth = GetComponent<PlayerStatistics>();
        DistantHealth = GetComponent<PlayerDistantHealth>();
    }

    public override void Initialize(ushort id, string newName, int teamId)
    {
        base.Initialize(id, newName, teamId);

        if (IsLocalPlayer)
        {
            EnableInput(false);
        }
        else
        {
            _pseudoText.text = newName;
        }
    }

    public void EnableInput(bool value)
    {
        _input.IsInputLock = !value;
        _cameraInput.IsInputLock = !value;
        Aim.CanShoot = value;
    }
}