using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationHelper : MonoBehaviour
{
    public HarvestSystem harvestSystem;

    public FarmNPC npc;

    public void PlayerSwingSound()
    {
        if(harvestSystem != null)
        {
            harvestSystem.PlaySwingSound();
        }
    }

    public void HarvestPlants()
    {
        if(harvestSystem != null)
        {
            harvestSystem.HarvestPlants();
        }
        else if (npc != null)
        {
            npc.TriggerHarvest();
        }
    }

    public void EndNPCHarvestAnimation()
    {
        if(npc != null)
        {
            npc.EndHarvestAnimation();
        }
    }

    
}
