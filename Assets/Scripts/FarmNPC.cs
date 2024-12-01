using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class FarmNPC : MonoBehaviour
{
    public enum State
    {
        Idle,
        MovingToSpawnpoint,
        MovingToSellPoint,
        MovingToPlant,
        Harvest,
        Dancing
    }
    [Header("State")]
    public State currentState;
    Plant target;
    bool isHarvesting = false;

    [Header("References")]
    [SerializeField] string plantName = "Corn";
    public Plant[] possibleTargets;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] SellingZone sellingZone;

    Vector3 spawnPoint;
    Quaternion spawnRotation;


    [Header("Corn Settings")]
    [SerializeField] int cornCounter = 0;
    [SerializeField] int maxCarrieableCorn = 1;
    [SerializeField] GameObject cornPrefab;
    [SerializeField] Transform cornBackpack;
    List<GameObject> cornObjects = new();

    [Header("Animation Handling")]
    public float animationDamping = 1f;
    Coroutine decreasingDanceLayerWeightCoroutine;
    Coroutine decreasingHarvestLayerWeightCoroutine;
    Coroutine increasingHarvestLayerWeightCoroutine;

    void Start()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;

        SwitchState(State.Dancing);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                if (target != null)
                {
                    SwitchState(State.MovingToPlant);
                }
                else
                {
                    FindNewTarget();
                }

                break;

            case State.MovingToSpawnpoint:
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    transform.rotation = spawnRotation;
                    SwitchState(State.Idle);
                }
                break;

            case State.MovingToSellPoint:
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        SellPlants();
                        SwitchState(State.Idle);
                    }
                    break;
                }

            case State.MovingToPlant:
                if (target == null || target.state != Plant.plantState.READY)
                {
                    FindNewTarget();
                    return;
                }
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    // Arrived at plant
                    SwitchState(State.Harvest);
                }
                break;

            case State.Harvest:
                if (!isHarvesting)
                {
                    if (target != null && target.state == Plant.plantState.READY)
                    {
                        animator.SetBool("canHarvest", true);
                        isHarvesting = true;
                    }
                    else
                    {
                        FindNewTarget();
                    }
                }
                break;

            default:
                break;
        }

        float velocity = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", velocity * agent.speed);
    }

    [SerializeField] float harvestAnimationDamping;
    void SwitchState(State newState)
    {

        if (newState == currentState) return;
        
        if (currentState == State.Harvest)
        {
            animator.SetBool("canHarvest", false);
            isHarvesting = false;



            if(decreasingHarvestLayerWeightCoroutine == null)
            {
                decreasingHarvestLayerWeightCoroutine = StartCoroutine(DecreaseLayerWeightInAnimator(1, harvestAnimationDamping));
            }
        }
        
        if (currentState == State.Dancing)
        {
            
        }

        switch (newState)
        {
            case State.Idle:
                break;

            case State.MovingToSpawnpoint:
                agent.enabled = true;
                agent.SetDestination(spawnPoint);
                break;

            case State.MovingToSellPoint:
                agent.enabled = true;
                agent.SetDestination(sellingZone.transform.position);
                break;

            case State.MovingToPlant:
                agent.enabled = true;
                agent.SetDestination(target.transform.position);
                break;

            case State.Harvest:
                agent.SetDestination(transform.position);
                agent.enabled = false;

                if (increasingHarvestLayerWeightCoroutine == null)
                {
                    decreasingHarvestLayerWeightCoroutine = null;
                    increasingHarvestLayerWeightCoroutine = StartCoroutine(IncreaseLayerWeightInAnimator(1, harvestAnimationDamping));
                }

                animator.SetBool("canHarvest", true);
                isHarvesting = true;

                RotateTowardsTarget();
                break;

            case State.Dancing:
                animator.SetLayerWeight(2, 1f);
                break;

            default:
                break;
        }

        currentState = newState;
    }

    private IEnumerator IncreaseLayerWeightInAnimator(int index, float speed)
    {
        float targetValue = 1f; // Ziel ist 1
        float currentValue = animator.GetLayerWeight(index);

        while (!Mathf.Approximately(currentValue, targetValue))
        {
            // Smoothly decrease towards the target value
            currentValue += speed * Time.deltaTime;
            currentValue = Mathf.Min(currentValue, targetValue); // Clamp to target

            animator.SetLayerWeight(index, currentValue);

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set
        animator.SetLayerWeight(index, targetValue);

        // Mark coroutine as finished
        if (index == 1)
        {
            increasingHarvestLayerWeightCoroutine = null;
        }
    }

    private IEnumerator DecreaseLayerWeightInAnimator(int index, float speed)
    {
        float targetValue = 0f; // Ziel ist 0
        float currentValue = animator.GetLayerWeight(index);

        while (!Mathf.Approximately(currentValue, targetValue))
        {
            // Smoothly decrease towards the target value
            currentValue -= speed * Time.deltaTime;
            currentValue = Mathf.Max(currentValue, targetValue); // Clamp to target

            animator.SetLayerWeight(index, currentValue);

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set
        animator.SetLayerWeight(index, targetValue);

        // Mark coroutine as finished
        if(index == 1)
        {
            decreasingHarvestLayerWeightCoroutine = null;
        }
        else if (index == 2)
        {
            decreasingDanceLayerWeightCoroutine = null;

            SwitchState(State.Idle);
        }
    }

    void FindNewTarget()
    {
        List<Plant> plants = new List<Plant>(possibleTargets);
        ShuffleList(plants);

        foreach (Plant plant in plants)
        {
            if (plant.state == Plant.plantState.READY)
            {
                target = plant;
                SwitchState(State.MovingToPlant);
                return;
            }
        }

        SwitchState(State.MovingToSpawnpoint);
    }

    public void TriggerHarvest()
    {
        if (target == null || target.state != Plant.plantState.READY)
        {
            FindNewTarget();
            return;
        }

        target.Harvest(this);

        if (target.state != Plant.plantState.READY)
        {
            SwitchState(State.Idle);

            target = null;

            isHarvesting = false;
            animator.SetBool("canHarvest", false);

        }

        if (cornCounter > 0)
        {
            InitializeCornObjects();

            if(cornCounter >= maxCarrieableCorn)
            {
                SwitchState(State.MovingToSellPoint);

                target = null;

                isHarvesting = false;
                animator.SetBool("canHarvest", false);
            }

        }
    }

    public void EndHarvestAnimation()
    {
        animator.SetBool("canHarvest", false);
        isHarvesting = false;
    }

    public void EndDanceAnimation()
    {
        if (decreasingDanceLayerWeightCoroutine == null)
        {
            decreasingDanceLayerWeightCoroutine = StartCoroutine(DecreaseLayerWeightInAnimator(2, animationDamping));
        }
    }



    void SellPlants()
    {
        if(plantName == "Corn")
        {
            sellingZone.SellPlant(plantName, cornCounter);

            cornCounter = 0;

            for (int i = 0; i < cornObjects.Count; i++)
            {
                Destroy(cornObjects[i]);
            }
            cornObjects.Clear();

            SwitchState(State.Idle);
        }
        else
        {
            Debug.LogError($"NPC tries to sell an unknown plant named {plantName}!");
        }
    }

    void InitializeCornObjects()
    {
        foreach (GameObject cornObject in cornObjects)
        {
            Destroy(cornObject);
        }

        cornObjects.Clear();

        for (int i = 0; i < cornCounter; i++)
        {
            GameObject corn = Instantiate(cornPrefab, cornBackpack);

            corn.transform.localPosition = new Vector3(0, -0.25f + 0.15f * i, 0);
            corn.transform.localRotation = Quaternion.Euler(0, -90f, 0);

            cornObjects.Add(corn);
        }
    }
    
    void ShuffleList(List<Plant> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Plant value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void RotateTowardsTarget()
    {
        if (target == null) return;

        // Calculate the direction vector from this GameObject to the target
        Vector3 directionToTarget = target.transform.position - transform.position;

        // Calculate the rotation required to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public void AddPlant(string plantName)
    {
        switch (plantName)
        {
            case "Corn":

                cornCounter++;

                // Initialize corn object
                InitializeCornObjects();

                break;
            default:
                Debug.Log($"Trying to add '{plantName}', but it's not found. Spelling error?");
                break;
        }
    }
}
