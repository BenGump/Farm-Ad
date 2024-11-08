using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingZone : MonoBehaviour
{
    public string sellingPlant = "Corn";
    public int sellingPrice = 1;

    [SerializeField] Inventory playerInventory;
    [SerializeField] MoneyZone moneyZone;
    
    [SerializeField] bool isPlayerInside = false;

    public float sellingInterval = 0.5f;

    float timer;

    private void Update()
    {
        if(isPlayerInside)
        {
            if(playerInventory.HasPlant(sellingPlant))
            {
                timer += Time.deltaTime;

                if(timer >= sellingInterval)
                {
                    playerInventory.RemovePlant(sellingPlant);

                    TransferPlantToCash(1);

                    timer = 0f;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    public void TransferPlantToCash(int amountOfPlants)
    {
        moneyZone.AddCashToStock(amountOfPlants * sellingPrice);
    }
}
