using UnityEngine;

public class PlayerAnimationHelper : MonoBehaviour
{
    public HarvestSystem harvestSystem;

    public FarmNPC npc;

    public void EndDance()
    {
        if(npc != null)
        {
            npc.EndDanceAnimation();
        }
    }

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
