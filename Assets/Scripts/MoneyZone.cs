using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyZone : MonoBehaviour
{
    [SerializeField] int currentMoneyStock = 0;

    [SerializeField] Inventory playerInventory;

    [SerializeField] bool isPlayerInside = false;

    public float moneyPickupInterval = 0.5f;

    float timer;


    public void AddCashToStock(int amount)
    {
        currentMoneyStock += amount;
    }

    private void Update()
    {
        if (isPlayerInside)
        {
            timer += Time.deltaTime;

            if (timer >= moneyPickupInterval)
            {
                playerInventory.AddCash(currentMoneyStock);
                currentMoneyStock = 0;

                timer = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
