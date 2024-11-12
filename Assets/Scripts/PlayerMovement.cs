using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // VARIABLES
    [Header("Speed")]
    public bool canMove = true;
    [SerializeField] private float moveSpeed = 10f;

    [Header("Rotation")]
    public float rotationSpeed = 8f;
    public LayerMask rotateMouseLayer; // Layer that gets hit by the mouse ray to rotate to that point

    private float moveX;
    private float moveZ;
    private Vector3 moveDirection;
    private Vector3 velocity;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundMask;
    private float groundCheckDistance = 0.2f;
    private float gravity = -9.81f;

    [Header("Animation Handling")]
    public float animationDamping = 1f;
    Coroutine animationDampingCoroutine;


    // REFERENCES
    private CharacterController controller;
    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {

        if (canMove)
        {
            SetMovementInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Move();
        }

        if(moveDirection == Vector3.zero)
        {
            animator.SetFloat("Speed", 0f);
            
            // Reset Coroutine
            if(animationDampingCoroutine != null)
            {
                StopCoroutine(animationDampingCoroutine);
                animationDampingCoroutine = null;
            }
        }
        else
        {
            //animator.SetFloat("Speed", moveDirection.magnitude);
            //Debug.Log(moveDirection.magnitude);

            if(animationDampingCoroutine == null)
            {
                // Start Coroutine if not already done
                animationDampingCoroutine = StartCoroutine(IncreaseSpeedValueInAnimator());
            }
        }

    }

    private IEnumerator IncreaseSpeedValueInAnimator()
    {
        float currentValue = animator.GetFloat("Speed");
        float targetValue = moveDirection.magnitude;

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


    public void Move()
    {
        CheckIfGrounded();

        CalculateGravity();

        CalculateMovementDirection();

        RotateIfMoving();

        //RotateToMousePosition();

        ApplyMovement();

        ApplyGravity();
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
    }

    void CalculateGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    public void SetMovementInput(float x, float z)
    {
        moveX = x;
        moveZ = z;
    }

    void CalculateMovementDirection()
    {
        moveDirection = new Vector3(moveX, 0, moveZ);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
    }
    void RotateIfMoving()
    {
        if (moveDirection != Vector3.zero)
        {
            RotateToMoveDirection();
        }
    }

    public void ApplyMovement()
    {
        moveDirection *= moveSpeed;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void RotateToMoveDirection()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotationSpeed);
    }

    /*

    public void RotateToMousePosition()
    {
        // Get the mouse position in the world
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        // Calculate direction from player to mouse position
        Vector3 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        directionToMouse.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToMouse), Time.deltaTime * rotationSpeed);
    }

    public Vector3 GetMouseWorldPosition()
    {
        // Convert mouse position to world position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rotateMouseLayer))
        {
            return hit.point;
        }
        return ray.GetPoint(100f); // Default far away point if nothing is hit
    }

    */
}

