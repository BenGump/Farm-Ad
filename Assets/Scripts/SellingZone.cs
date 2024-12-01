using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SellingZone : MonoBehaviour
{
    public string sellingPlant = "Corn";
    public int sellingPrice = 1;

    [SerializeField] Inventory playerInventory;

    [SerializeField] AudioClip sellingSound;
    AudioSource audioSource;


    bool isPlayerInside = false;

    float sellingInterval = 0.1f;

    float timer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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

            audioSource.PlayOneShot(sellingSound);
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
