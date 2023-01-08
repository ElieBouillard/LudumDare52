using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int BagSize;
    public int BottleSize;

    public int WaterAmount;
    public int FoodAmount;

    public float WaterValueWhenDrink;
    public float FoodValueWhenEat;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            
        }
    }
}

public enum ItemType
{
    Food,
    Water,
}