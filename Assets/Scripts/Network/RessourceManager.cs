using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RessourceManager : Singleton<RessourceManager>
{
    public Ressource[] Ressources;

    [SerializeField] private TMP_Text _ferText;
    [SerializeField] private TMP_Text _plasticText;
    [SerializeField] private TMP_Text _energyText;

    private Dictionary<RessourceType, int> RessourceInventory = new Dictionary<RessourceType, int>();

    private void Start()
    {
        RessourceInventory.Add(RessourceType.Fer,0);
        RessourceInventory.Add(RessourceType.Plastic,0);
        RessourceInventory.Add(RessourceType.Energy,0);
        
        for (int i = 0; i < Ressources.Length; i++)
        {
            Ressources[i].Id = (ushort)i;
        }
    }

    public void AddRessource(RessourceType ressourceType, int amount)
    {
        RessourceInventory[ressourceType] += amount;
        
        UpdateInterface();
    }

    private void UpdateInterface()
    {
        _ferText.text = $"Fer : {RessourceInventory[RessourceType.Fer]}";
        _plasticText.text = $"Plastic : {RessourceInventory[RessourceType.Plastic]}";
        _energyText.text = $"Energy : {RessourceInventory[RessourceType.Energy]}";
    }
}
