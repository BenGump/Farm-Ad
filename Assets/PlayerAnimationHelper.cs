using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHelper : MonoBehaviour
{
    public HarvestSystem harvestSystem;

    public void HarvestPlants()
    {
        harvestSystem.HarvestPlants();
    }
}
