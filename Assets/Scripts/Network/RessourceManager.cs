using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RessourceManager : Singleton<RessourceManager>
{
    public Ressource[] Ressources;

    [Header("Team")]
    [SerializeField] private TMP_Text _ferTeamText;
    [SerializeField] private TMP_Text _plasticTeamText;
    [SerializeField] private TMP_Text _energyTeamText;

    [Header("Personal")]
    [SerializeField] private TMP_Text _ferText;
    [SerializeField] private TMP_Text _plasticText;
    [SerializeField] private TMP_Text _energyText;
    
    public Dictionary<RessourceType, int> TeamRessourceInventory { private set; get; } = new Dictionary<RessourceType, int>();

    public Dictionary<RessourceType, int> RessourceInventory { private set; get; } = new Dictionary<RessourceType, int>();


    private void Start()
    {
        TeamRessourceInventory.Add(RessourceType.Fer,0);
        TeamRessourceInventory.Add(RessourceType.Plastic,0);
        TeamRessourceInventory.Add(RessourceType.Energy,0);
        
        ResetRessourcesInventory();
        
        for (int i = 0; i < Ressources.Length; i++)
        {
            Ressources[i].Id = (ushort)i;
        }
    }

    public void AddTeamRessource(RessourceType ressourceType, int amount)
    {
        TeamRessourceInventory[ressourceType] += amount;
        
        UpdateTeamInterface();
    }

    public void AddRessource(RessourceType ressourceType, int amount)
    {
        RessourceInventory[ressourceType] += amount;
        
        UpdateInterface();
    }
    
    public void ResetRessourcesInventory()
    {
        RessourceInventory.Add(RessourceType.Fer,0);
        RessourceInventory.Add(RessourceType.Plastic,0);
        RessourceInventory.Add(RessourceType.Energy,0);
    }
    private void UpdateTeamInterface()
    {
        _ferTeamText.text = TeamRessourceInventory[RessourceType.Fer].ToString();
        _plasticTeamText.text = TeamRessourceInventory[RessourceType.Plastic].ToString();
        _energyTeamText.text = TeamRessourceInventory[RessourceType.Energy].ToString();
    }

    private void UpdateInterface()
    {        
        _ferText.text = RessourceInventory[RessourceType.Fer].ToString();
        _plasticText.text = RessourceInventory[RessourceType.Plastic].ToString();
        _energyText.text = RessourceInventory[RessourceType.Energy].ToString();
    }
}
