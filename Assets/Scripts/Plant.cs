using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Inventory playerInventory;
    public HarvestSystem harvestSystem;

    public float growBackTime = 2f;

    [SerializeField] int harvestAttemptsNeeded = 2;
    [SerializeField] int currentHarvestAttempt = 0;

    MeshRenderer meshRenderer;
    BoxCollider boxCollider;
    Animator animator;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
    }

    public enum plantState
    {
        GROWING,
        READY
    }
    public plantState state;

    public string plantName;

    public void Harvest(FarmNPC npc = null)
    {
        if(state == plantState.READY)
        {
            currentHarvestAttempt++;

            animator.SetTrigger("Shake");

            if(currentHarvestAttempt >= harvestAttemptsNeeded)
            {
                // Add to inventory from NPC or the Players inventory
                if(npc)
                {
                    if(plantName == "Corn")
                    {
                        npc.cornCounter++;
                    }
                    else
                    {
                        Debug.Log("Plant name doesnt appear in NPC inventory code, it's not hardcoded");
                    }
                }
                else
                {
                    playerInventory.AddPlant(plantName);
                }

                boxCollider.enabled = false;
                meshRenderer.enabled = false;

                state = plantState.GROWING;
                float timeToRegrow = UnityEngine.Random.Range(growBackTime - .2f, growBackTime + .2f);
                Invoke("ResetPlant", timeToRegrow);

                //harvestSystem.RemoveDetectedPlant(gameObject);
            }
        }
    }

    private void ResetPlant()
    {
        state = plantState.READY;
        currentHarvestAttempt = 0;

        boxCollider.enabled = true;
        meshRenderer.enabled = true;

        animator.SetTrigger("Spawn");
    }
}
