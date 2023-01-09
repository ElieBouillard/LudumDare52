using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RessourceManager : Singleton<RessourceManager>
{
    public List<Ressource> Ressources;

    [Header("Team")]
    [SerializeField] private TMP_Text _ferTeamText;
    [SerializeField] private TMP_Text _plasticTeamText;
    [SerializeField] private TMP_Text _energyTeamText;

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

        List<Ressource> ressourcesToDelete = new List<Ressource>();

        for (int i = 0; i < ferAmount; i++)
        {
            foreach (var ressource in Ressources)
            {
                if(!ressource.IsCollected) continue;

                if(ressource.RessourceType != RessourceType.Fer) continue;

                if (ressource.PlayerCollector.GetId == id)
                {
                    ressourcesToDelete.Add(ressource);
                }
            }
        }
        
        for (int i = 0; i < plasticAmount; i++)
        {
            foreach (var ressource in Ressources)
            {
                if(!ressource.IsCollected) continue;

                if(ressource.RessourceType != RessourceType.Plastic) continue;

                if (ressource.PlayerCollector.GetId == id)
                {
                    ressourcesToDelete.Add(ressource);
                }
            }
        }
        
        for (int i = 0; i < energyAmount; i++)
        {
            foreach (var ressource in Ressources)
            {
                if(!ressource.IsCollected) continue;

                if(ressource.RessourceType != RessourceType.Energy) continue;

                if (ressource.PlayerCollector.GetId == id)
                {
                    ressourcesToDelete.Add(ressource);
                }
            }
        }

        for (int i = 0; i < ressourcesToDelete.Count; i++)
        {
            ressourcesToDelete[i].CollectedToBase();
        }
        
        if(NetworkManager.Instance.LocalPlayer.GetId == id)
            UpdatePlayerInterface();
        
        if(NetworkManager.Instance.LocalPlayer.TeamId == teamId)
            UpdateTeamInterface(teamId);
    }

    public void LocalAddRessourceToBase()
    {
        ushort localId = NetworkManager.Instance.LocalPlayer.GetId;
        
        NetworkManager.Instance.ClientMessages.SendOnDropRessources(PlayerInventories[localId].FerAmount,
            PlayerInventories[localId].PlasticAmount, PlayerInventories[localId].EnergyAmount);

        AddRessourceToBase(localId, PlayerInventories[localId].FerAmount, PlayerInventories[localId].PlasticAmount, PlayerInventories[localId].EnergyAmount);
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
        if (teamId == 0)
        {
            _ferTeamText.text = Team0RessourceInventory[RessourceType.Fer].ToString();
            _plasticTeamText.text = Team0RessourceInventory[RessourceType.Plastic].ToString();
            _energyTeamText.text = Team0RessourceInventory[RessourceType.Energy].ToString();
        }
        else if(teamId == 1)
        {
            _ferTeamText.text = Team1RessourceInventory[RessourceType.Fer].ToString();
            _plasticTeamText.text = Team1RessourceInventory[RessourceType.Plastic].ToString();
            _energyTeamText.text = Team1RessourceInventory[RessourceType.Energy].ToString();
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