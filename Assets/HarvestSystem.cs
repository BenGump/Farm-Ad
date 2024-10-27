using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestSystem : MonoBehaviour
{
    [SerializeField] bool isHarvesting = false;
    [SerializeField] bool canHarvestPlant = false;

    Animator animator;

    [SerializeField] LayerMask plantLayer;
    [SerializeField] float checkInterval = .2f;

    List<Plant> detectedPlants = new List<Plant>();

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();    
    }

    void Start()
    {
        InvokeRepeating("CheckForHarvestablePlants", checkInterval, checkInterval);    
    }

    void CheckForHarvestablePlants()
    {
        if(detectedPlants.Count == 0)
        {
            canHarvestPlant = false;

            if (isHarvesting)
                StopHarvesting();
            return;
        }


        for (int i = 0; i < detectedPlants.Count; i++)
        {
            if (detectedPlants[i].state == Plant.plantState.READY)
            {
                canHarvestPlant = true;
                break;
            }
            else
            {
                canHarvestPlant = false;
            }
        }

        if(canHarvestPlant)
        {
            StartHarvesting();
        }
        else
        {
            StopHarvesting();
        }
    }

    public void StartHarvesting()
    {
        if(!isHarvesting)
        {
            isHarvesting = true;

            animator.SetLayerWeight(1, 1f);
            animator.SetBool("canHarvest", true);
        }
    }

    // More like an event but its easier like that right now
    // Gets called by the player harvest animation (in the middle of the swing)
    public void HarvestPlants()
    {
        for (int i = 0; i < detectedPlants.Count; i++)
        {
            if (detectedPlants[i].state == Plant.plantState.READY)
            {
                detectedPlants[i].Harvest();
            }
        }
    }

    public void StopHarvesting()
    {
        isHarvesting = false;
        animator.SetBool("canHarvest", false);
        animator.SetLayerWeight(1, 0f);
    }

    public void AddDetectedPlant(GameObject plantObject)
    {
        if(plantObject.TryGetComponent<Plant>(out Plant plant))
        {
            if(!detectedPlants.Contains(plant))
            {
                detectedPlants.Add(plant);
            }
            else
            {
                Debug.Log("Detected Plant already on list.");
            }

        }
        else
        {
            Debug.Log("Plant Object has no plant script attached to it.");
        }
    }

    public void RemoveDetectedPlant(GameObject plantObject)
    {
        if (plantObject.TryGetComponent<Plant>(out Plant plant))
        {
            if (!detectedPlants.Contains(plant))
            {
                Debug.Log("Plant is not on the list and therefore can't be removed");
            }
            else
            {
                detectedPlants.Remove(plant);
            }

        }
        else
        {
            Debug.Log("Plant Object has no plant script attached to it.");
        }
    }
}
