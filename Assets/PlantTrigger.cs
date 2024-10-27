using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTrigger : MonoBehaviour
{
    public HarvestSystem harvestSystem;

    private void OnTriggerEnter(Collider other)
    {
        harvestSystem.AddDetectedPlant(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        harvestSystem.RemoveDetectedPlant(other.gameObject);
    }

}
