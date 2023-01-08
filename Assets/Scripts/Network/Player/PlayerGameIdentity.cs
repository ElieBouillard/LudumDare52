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
    
    public PlayerStatistics _localHealth { private set; get; }
    public PlayerDistantHealth _distantHealth { private set; get; }
    public PlayerAim Aim { private set; get; }

    private CharacterKeyboardInput _input;

    private void Awake()
    {
        MovementReceiver = GetComponent<MovementReceiver>();
        Animations = GetComponentInChildren<PlayerDistantAnimations>();
        Aim = GetComponent<PlayerAim>();
        _input = GetComponent<CharacterKeyboardInput>();
        _localHealth = GetComponent<PlayerStatistics>();
        _distantHealth = GetComponent<PlayerDistantHealth>();
    }

    public override void Initialize(ushort id, string newName)
    {
        base.Initialize(id, newName);

        if (_pseudoText == null) return;
        _pseudoText.text = newName;
    }

    public void EnableInput(bool value)
    {
        _input.IsInputLock = !value;
        Aim.CanShoot = value;
    }
}