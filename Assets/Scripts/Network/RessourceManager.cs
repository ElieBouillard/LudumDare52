using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RessourceManager : Singleton<RessourceManager>
{
    public List<Ressource> Ressources;

    public int FerAmountToWin = 2;
    public int PlasticAmountToWin = 2;
    public int EnergyAmountToWin = 0;
    
    [Header("Team")]
    [SerializeField] private TMP_Text _ferTeam0Text;
    [SerializeField] private TMP_Text _plasticTeam0Text;
    [SerializeField] private TMP_Text _energyTeam0Text;

    [SerializeField] private TMP_Text _ferTeam1Text;
    [SerializeField] private TMP_Text _plasticTeam1Text;
    [SerializeField] private TMP_Text _energyTeam1Text;
    
    [Header("Personal")]
    [SerializeField] private TMP_Text _ferText;
    [SerializeField] private TMP_Text _plasticText;
    [SerializeField] private TMP_Text _energyText;
    
    public Dictionary<RessourceType, int> Team0RessourceInventory { private set; get; } = new Dictionary<RessourceType, int>();
    public Dictionary<RessourceType, int> Team1RessourceInventory { private set; get; } = new Dictionary<RessourceType, int>();
    public Dictionary<ushort, PlayerInventory> PlayerInventories { private set; get; } = new Dictionary<ushort, PlayerInventory>();

private void Start()
    {
        Team0RessourceInventory.Add(RessourceType.Fer,0);
        Team0RessourceInventory.Add(RessourceType.Plastic,0);
        Team0RessourceInventory.Add(RessourceType.Energy,0);

        Team1RessourceInventory.Add(RessourceType.Fer,0);
        Team1RessourceInventory.Add(RessourceType.Plastic,0);
        Team1RessourceInventory.Add(RessourceType.Energy,0);
        
        foreach (var playerId in NetworkManager.Instance.Players.Keys)
        {
            PlayerInventories.Add(playerId, new PlayerInventory());
        }

        for (int i = 0; i < Ressources.Count; i++)
        {
            Ressources[i].Id = (ushort)i;
        }
        
        _ferTeam0Text.text =$"0/{FerAmountToWin}";
        _plasticTeam0Text.text = $"0/{PlasticAmountToWin}";
        _energyTeam0Text.text = $"0/{EnergyAmountToWin}";
        _ferTeam1Text.text = $"0/{FerAmountToWin}";
        _plasticTeam1Text.text = $"0/{PlasticAmountToWin}";
        _energyTeam1Text.text = $"0/{EnergyAmountToWin}";
    }

    public void AddRessourceToPlayer(ushort id, RessourceType ressourceType, int amount)
    {
        PlayerInventories[id].AddRessource(ressourceType, amount);

        if (id != NetworkManager.Instance.LocalPlayer.GetId) return;
        
        UpdatePlayerInterface();
    }

    public void AddRessourceToBase(ushort id, int ferAmount, int plasticAmount, int energyAmount)
    {
        int teamId = -1;
        
        if (NetworkManager.Instance.Team0.ContainsKey(id))
        {
            Team0RessourceInventory[RessourceType.Fer]+= ferAmount;
            Team0RessourceInventory[RessourceType.Plastic]+= plasticAmount;
            Team0RessourceInventory[RessourceType.Energy]+= energyAmount;
            teamId = 0;
        }
        else if(NetworkManager.Instance.Team1.ContainsKey(id))
        {
            Team1RessourceInventory[RessourceType.Fer]+= ferAmount;
            Team1RessourceInventory[RessourceType.Plastic]+= plasticAmount;
            Team1RessourceInventory[RessourceType.Energy]+= energyAmount;
            teamId = 1;
        }

        PlayerInventories[id].FerAmount -= ferAmount;
        PlayerInventories[id].PlasticAmount -= plasticAmount;
        PlayerInventories[id].EnergyAmount -= energyAmount;

        for (int i = 0; i < ferAmount; i++)
        {
            foreach (var ressource in Ressources)
            {
                if(!ressource.IsCollected) continue;

                if(ressource.IsCollectedInBase) continue;
                
                if(ressource.RessourceType != RessourceType.Fer) continue;

                if (ressource.PlayerCollector.GetId == id)
                {
                    ressource.CollectedToBase();
                    break;
                }
            }
        }
        
        for (int i = 0; i < plasticAmount; i++)
        {
            foreach (var ressource in Ressources)
            {
                if(!ressource.IsCollected) continue;

                if(ressource.IsCollectedInBase) continue;
                
                if(ressource.RessourceType != RessourceType.Plastic) continue;

                if (ressource.PlayerCollector.GetId == id)
                {
                    ressource.CollectedToBase();
                    break;
                }
            }
        }
        
        for (int i = 0; i < energyAmount; i++)
        {
            foreach (var ressource in Ressources)
            {
                if(!ressource.IsCollected) continue;

                if(ressource.IsCollectedInBase) continue;
                
                if(ressource.RessourceType != RessourceType.Energy) continue;

                if (ressource.PlayerCollector.GetId == id)
                {
                    ressource.CollectedToBase();
                    break;
                }
            }
        }

        if(NetworkManager.Instance.LocalPlayer.GetId == id)
            UpdatePlayerInterface();
        
        UpdateTeamInterface(teamId);

        CheckVictory();
    }

    private void CheckVictory()
    {
        if (Team0RessourceInventory[RessourceType.Fer] >= FerAmountToWin &&
            Team0RessourceInventory[RessourceType.Plastic] >= PlasticAmountToWin &&
            Team0RessourceInventory[RessourceType.Energy] >= EnergyAmountToWin)
        {
            GameManager.Instance.SetEndGame(NetworkManager.Instance.LocalPlayer.TeamId == 0);
        }
        
        if (Team1RessourceInventory[RessourceType.Fer] >= FerAmountToWin &&
            Team1RessourceInventory[RessourceType.Plastic] >= PlasticAmountToWin &&
            Team1RessourceInventory[RessourceType.Energy] >= EnergyAmountToWin)
        {
            GameManager.Instance.SetEndGame(NetworkManager.Instance.LocalPlayer.TeamId == 1);
        }
    }

    public void LocalAddRessourceToBase()
    {
        ushort localId = NetworkManager.Instance.LocalPlayer.GetId;

        Dictionary<RessourceType, int> teamInventory = NetworkManager.Instance.LocalPlayer.TeamId == 0
            ? Team0RessourceInventory : Team1RessourceInventory;

        int ferToDrop = 0;
        int plasticToDrop = 0;
        int energyToDrop = 0;
        
        if (teamInventory[RessourceType.Fer] >= FerAmountToWin)
        {
            ferToDrop = 0;
        }
        else if(PlayerInventories[localId].FerAmount + teamInventory[RessourceType.Fer] >= FerAmountToWin)
        {
            ferToDrop = FerAmountToWin - teamInventory[RessourceType.Fer];
        }
        else
        {
            ferToDrop = PlayerInventories[localId].FerAmount;
        }

        if (teamInventory[RessourceType.Plastic] >= PlasticAmountToWin)
        {
            plasticToDrop = 0;
        }
        else if(PlayerInventories[localId].PlasticAmount + teamInventory[RessourceType.Plastic] >= PlasticAmountToWin)
        {
            plasticToDrop = PlasticAmountToWin - teamInventory[RessourceType.Plastic];
        }
        else
        {
            plasticToDrop = PlayerInventories[localId].PlasticAmount;
        }
        
        if (teamInventory[RessourceType.Energy] >= EnergyAmountToWin)
        {
            energyToDrop = 0;
        }
        else if(PlayerInventories[localId].EnergyAmount + teamInventory[RessourceType.Energy] >= EnergyAmountToWin)
        {
            energyToDrop = EnergyAmountToWin - teamInventory[RessourceType.Energy];
        }
        else
        {
            energyToDrop = PlayerInventories[localId].EnergyAmount;
        }

        NetworkManager.Instance.ClientMessages.SendOnDropRessources(ferToDrop,
            plasticToDrop, energyToDrop);

        AddRessourceToBase(localId, ferToDrop, plasticToDrop, energyToDrop);
    }
    
    public void Death(ushort playerId)
    {
        foreach (var ressource in Ressources)
        {
            if (!ressource.IsCollected) continue;

            if (ressource.IsCollectedInBase) continue;
            
            if (ressource.PlayerCollector.GetId == playerId)
            {
                ressource.InitializeTravelToStartPos();
            }
        }
        
        PlayerInventories[playerId].Reset();
        
        if(playerId != NetworkManager.Instance.LocalPlayer.GetId) return;

        UpdatePlayerInterface();
    }


    private void UpdatePlayerInterface()
    {
        ushort id = NetworkManager.Instance.LocalPlayer.GetId;

        _ferText.text = PlayerInventories[id].FerAmount.ToString();
        _plasticText.text = PlayerInventories[id].PlasticAmount.ToString();
        _energyText.text = PlayerInventories[id].EnergyAmount.ToString();
    }
    
    private void UpdateTeamInterface(int teamId)
    {
        if (teamId == NetworkManager.Instance.LocalPlayer.TeamId)
        {
            if (teamId == 0)
            {
                _ferTeam0Text.text =$"{Team0RessourceInventory[RessourceType.Fer].ToString()}/{FerAmountToWin}";
                _plasticTeam0Text.text = $"{Team0RessourceInventory[RessourceType.Plastic].ToString()}/{PlasticAmountToWin}";
                _energyTeam0Text.text = $"{Team0RessourceInventory[RessourceType.Energy].ToString()}/{EnergyAmountToWin}";
            }
            else if(teamId == 1)
            {
                _ferTeam0Text.text = $"{Team1RessourceInventory[RessourceType.Fer].ToString()}/{FerAmountToWin}";
                _plasticTeam0Text.text = $"{Team1RessourceInventory[RessourceType.Plastic].ToString()}/{PlasticAmountToWin}";
                _energyTeam0Text.text = $"{Team1RessourceInventory[RessourceType.Energy].ToString()}/{EnergyAmountToWin}";
            }
        }
        else
        {
            if (teamId == 0)
            {
                _ferTeam1Text.text =$"{Team0RessourceInventory[RessourceType.Fer].ToString()}/{FerAmountToWin}";
                _plasticTeam1Text.text = $"{Team0RessourceInventory[RessourceType.Plastic].ToString()}/{PlasticAmountToWin}";
                _energyTeam1Text.text = $"{Team0RessourceInventory[RessourceType.Energy].ToString()}/{EnergyAmountToWin}";
            }
            else if(teamId == 1)
            {
                _ferTeam1Text.text = $"{Team1RessourceInventory[RessourceType.Fer].ToString()}/{FerAmountToWin}";
                _plasticTeam1Text.text = $"{Team1RessourceInventory[RessourceType.Plastic].ToString()}/{PlasticAmountToWin}";
                _energyTeam1Text.text = $"{Team1RessourceInventory[RessourceType.Energy].ToString()}/{EnergyAmountToWin}";
            }
        }
    }

    [ContextMenu("DebugPlayerInventories")]
    public void DebugPlayerInvetories()
    {
        foreach (var playerInventory in PlayerInventories)
        {
            Debug.Log($"Player{playerInventory.Key} : Fer {playerInventory.Value.FerAmount} : Plastic {playerInventory.Value.PlasticAmount} : Energy {playerInventory.Value.EnergyAmount}");
        }
        
        Debug.Log($"Team0 : Fer {Team0RessourceInventory[RessourceType.Fer]} : Plastic {Team0RessourceInventory[RessourceType.Plastic]} : Energy {Team0RessourceInventory[RessourceType.Energy]}");

        Debug.Log($"Team1 : Fer {Team1RessourceInventory[RessourceType.Fer]} : Plastic {Team1RessourceInventory[RessourceType.Plastic]} : Energy {Team1RessourceInventory[RessourceType.Energy]}");
    }
}

public class PlayerInventory
{
    public int FerAmount = 0;
    public int PlasticAmount = 0;
    public int EnergyAmount = 0;

    public void AddRessource(RessourceType ressource, int amount)
    {
        switch (ressource)
        {
            case RessourceType.Fer:
                FerAmount += amount;
                break;
            case RessourceType.Plastic:
                PlasticAmount += amount;
                break;
            case RessourceType.Energy:
                EnergyAmount += amount;
                break;
        }
    }

    public void Reset()
    {
        FerAmount = 0;
        PlasticAmount = 0;
        EnergyAmount = 0;
    }
}