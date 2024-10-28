using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class FarmNPC : MonoBehaviour
{
    public float detectionRadius = 5f;
    public LayerMask plantLayer;
    public List<GameObject> targetPlants = new List<GameObject>();

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 spawningPoint;
    private Quaternion spawningRotation;
    private GameObject currentTarget;
    private Plant currentPlant;

    [Header("Animation Handling")]
    public float animationDamping = 1f;
    Coroutine animationDampingCoroutine;

    private bool isHarvesting = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        spawningPoint = transform.position;
        spawningRotation = transform.rotation;

        FindNewTarget();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindNewTarget();
        }
        else
        {
            MoveToTarget();
        }

        if (agent.velocity == Vector3.zero)
        {
            animator.SetFloat("Speed", 0f);

            // Reset Coroutine
            if (animationDampingCoroutine != null)
            {
                StopCoroutine(animationDampingCoroutine);
                animationDampingCoroutine = null;
            }
        }
        else
        {
            // Start Coroutine
            if (animationDampingCoroutine == null)
            {
                // Start Coroutine if not already done
                animationDampingCoroutine = StartCoroutine(IncreaseSpeedValueInAnimator());
            }
        }
    }

    void FindNewTarget()
    {
        // Check for nearby plants using a physics overlap sphere
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, plantLayer);

        GameObject nearestPlant = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.TryGetComponent<Plant>(out Plant plant))
            {
                if (plant.state == Plant.plantState.GROWING) continue;

                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < minDistance)
                {
                    nearestPlant = hitCollider.gameObject;
                    minDistance = distance;
                }
            }
        }

        if (nearestPlant != null)
        {
            currentTarget = nearestPlant;
            currentPlant = nearestPlant.GetComponent<Plant>();
        }
        else if (targetPlants.Count > 0)
        {
            int loops = 0;
            bool foundTarget = false;

            while (!foundTarget && loops < 20)
            {
                currentTarget = targetPlants[Random.Range(0, targetPlants.Count)];
                currentPlant = currentTarget.GetComponent<Plant>();
                if (currentPlant.state == Plant.plantState.READY)
                {
                    foundTarget = true;
                    Debug.Log("Found target!");
                    return;
                }
                currentTarget = null;
                currentPlant = null;
                Debug.Log("No target found!");
                loops++;
            }

            // 10 loops and no target plant found

            GoToSpawnPoint();

        }
        else
        {
            // No plants available; return to spawn point
            GoToSpawnPoint();
        }
    }

    void MoveToTarget()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget > 1f)
        {
            isHarvesting = false;
            agent.SetDestination(currentTarget.transform.position);
        }
        else
        {
            FaceTarget(currentTarget.transform);
            Harvest();
        }
    }

    void GoToSpawnPoint()
    {
        currentTarget = null;
        currentPlant = null;
        isHarvesting = false;

        // Check if reached spawn point
        if (Vector3.Distance(transform.position, spawningPoint) < 0.1f)
        {
            FaceSpawnPoint();
        }
        else
        {
            agent.SetDestination(spawningPoint);
        }
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void FaceSpawnPoint()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, spawningRotation, Time.deltaTime * 5f);
    }

    public void Harvest()
    {
        if(!isHarvesting)
        {
            isHarvesting = true;
            animator.SetLayerWeight(1, 1f);
            animator.SetBool("canHarvest", true);
        }
        else
        {
            if(currentPlant.state == Plant.plantState.GROWING)
            {
                FindNewTarget();
            }
        }
    }

    public void HarvestPlants()
    {
        currentTarget.GetComponent<Plant>().Harvest();
    }

    #region Animation

    private IEnumerator IncreaseSpeedValueInAnimator()
    {
        float currentValue = animator.GetFloat("Speed");
        float targetValue = agent.speed;

        Debug.Log(targetValue);

        while (currentValue < targetValue)
        {
            // Increase the current value based on the increase speed and deltaTime
            currentValue += animationDamping * Time.deltaTime;

            // Clamp the current value to ensure it doesn't exceed the target
            currentValue = Mathf.Min(currentValue, targetValue);

            // Set the Animator parameter to the new value
            animator.SetFloat("Speed", currentValue);

            // Wait for the next frame before continuing the loop
            yield return null;
        }
    }

    #endregion
}
