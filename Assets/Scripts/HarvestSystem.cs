using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HarvestSystem : MonoBehaviour
{
    [SerializeField] bool isHarvesting = false;
    [SerializeField] bool canHarvestPlant = false;

    Animator animator;

    [SerializeField] LayerMask plantLayer;
    [SerializeField] float checkInterval = .2f;

    [SerializeField] GameObject scyteObject;
    [SerializeField] AudioClip swingSound;
    AudioSource audioSource;

    List<Plant> detectedPlants = new List<Plant>();

    [SerializeField] QuestManager questManager;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
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
                EndHarvesting();
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
            EndHarvesting();
        }
    }

    public void StartHarvesting()
    {
        if(!isHarvesting)
        {
            isHarvesting = true;

            scyteObject.SetActive(true);

            animator.SetLayerWeight(1, 1f);
            animator.SetBool("canHarvest", true);
        }
    }

    public void PlaySwingSound()
    {
        // Possible extension: Check if the plant is Corn for differentiating between Scyte swing and other future swings / harvests
        if(swingSound)
        {
            audioSource.PlayOneShot(swingSound);
        }
    }

    // More like an event but its easier like that right now
    // Gets called by the player harvest animation (in the middle of the swing)
    public void HarvestPlants()
    {
        List<Plant> plants = new List<Plant>(detectedPlants);

        for (int i = 0; i < plants.Count; i++)
        {
            if (plants[i].state == Plant.plantState.READY)
            {
                plants[i].Harvest();
            }
        }

        foreach (Plant plant in plants)
        {
            if(plant.state != Plant.plantState.READY)
            {
                detectedPlants.Remove(plant);
                questManager.IncreaseTotalCorn(1);
            }
        }
    }

    public void EndHarvesting()
    {
        StopHarvesting();
    }

    public void StopHarvesting()
    {
        isHarvesting = false;

        scyteObject.SetActive(false);

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
                //Debug.Log("Detected Plant already on list.");
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
            if (detectedPlants.Contains(plant))
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
