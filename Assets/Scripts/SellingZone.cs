using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingZone : MonoBehaviour
{
    public string sellingPlant = "Corn";
    public int sellingPrice = 1;

    [SerializeField] Inventory playerInventory;
    
    bool isPlayerInside = false;

    float sellingInterval = 0.1f;

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
                    
                    SellPlant(sellingPlant, 1);

                    timer = 0f;
                }
            }
        }
    }

    public void SellPlant(string plantName, int amount)
    {
        if(plantName == sellingPlant)
        {
            playerInventory.AddCash(sellingPrice * amount);
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
}
