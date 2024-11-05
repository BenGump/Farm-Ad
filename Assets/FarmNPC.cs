using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FarmNPC : MonoBehaviour
{
    public enum State
    {
        Idle,
        MovingToSpawnpoint,
        MovingToSellPoint,
        MovingToPlant,
        Harvest
    }
    [Header("State")]
    public State currentState;
    public Plant target;
    public bool isHarvesting = false;

    [Header("References")]
    public Plant[] possibleTargets;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform sellPoint;

    private Vector3 spawnPoint;
    private Quaternion spawnRotation;


    [Header("Corn Settings")]
    public int cornCounter = 0;
    public GameObject cornPrefab;
    public Transform cornBackpack;
    private GameObject cornObject;

    void Start()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;
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
                        SellPlant();
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

    void SwitchState(State newState)
    {
        if (newState == currentState) return;

        //Debug.Log($"Switching from {currentState} to {newState}");

        if (currentState == State.Harvest)
        {
            animator.SetBool("canHarvest", false);
            isHarvesting = false;
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
                agent.SetDestination(sellPoint.position);
                break;

            case State.MovingToPlant:
                agent.enabled = true;
                agent.SetDestination(target.transform.position);
                break;

            case State.Harvest:
                agent.SetDestination(transform.position);
                agent.enabled = false;

                animator.SetBool("canHarvest", true);
                isHarvesting = true;

                RotateTowardsTarget();
                break;

            default:
                break;
        }

        currentState = newState;
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
            InitializeCornObject();

            SwitchState(State.MovingToSellPoint);

            target = null;

            isHarvesting = false;
            animator.SetBool("canHarvest", false);
        }
    }

    public void EndHarvestAnimation()
    {
        animator.SetBool("canHarvest", false);
        isHarvesting = false;
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
                Debug.Log("Found a ready plant");
                SwitchState(State.MovingToPlant);
                return;
            }
        }

        Debug.Log("No readyish plant found");
        SwitchState(State.MovingToSpawnpoint);
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

    void SellPlant()
    {
        Debug.Log("Selling plant");

        cornCounter = 0;
        Destroy(cornObject);
        cornObject = null;

        SwitchState(State.Idle);


    }

    void InitializeCornObject()
    {
        if(cornObject == null)
        {
            GameObject corn = Instantiate(cornPrefab, cornBackpack);

            corn.transform.localPosition = new Vector3(0, -0.25f + 0.15f * cornCounter, 0);
            corn.transform.localRotation = Quaternion.Euler(0, -90f, 0);

            cornObject = corn;
        }
    }
}