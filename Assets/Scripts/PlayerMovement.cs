using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Type")]
    public bool isTouch = false;
    private bool touchCountReached = false;

    // VARIABLES
    [Header("Speed")]
    public bool canMove = true;
    [SerializeField] private float moveSpeed = 10f;

    [Header("Rotation")]
    public float rotationSpeed = 8f;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundMask;
    private float groundCheckDistance = 0.2f;
    private float gravity = -9.81f;

    [Header("Animation Handling")]
    public float animationDamping = 1f;
    public float joystickDeadzone = 0.1f; // Deadzone für den Joystick
    private float targetValue;

    // REFERENCES
    private CharacterController controller;
    private Animator animator;
    public Joystick joystick;

    private Vector3 moveDirection;
    private Vector3 velocity;

    private Coroutine animationDampingCoroutine;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(!touchCountReached)
        {
            if(Input.touchCount > 0)
            {
                Debug.Log("Touch detected!");
                touchCountReached = true;
                isTouch = true;
                joystick.gameObject.SetActive(true);
            }
        }
        if (!canMove) return;

        // Eingaben erfassen (Joystick oder Tastatur)
        float moveX, moveZ;

        if (isTouch)
        {
            moveX = joystick.Horizontal;
            moveZ = joystick.Vertical;

            // Deadzone anwenden
            if (Mathf.Abs(moveX) < joystickDeadzone) moveX = 0f;
            if (Mathf.Abs(moveZ) < joystickDeadzone) moveZ = 0f;
        }
        else
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveZ = Input.GetAxisRaw("Vertical");
        }

        moveDirection = new Vector3(moveX, 0, moveZ);

        // Bewegung normalisieren, falls nötig
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Zielwert für Animation setzen
        targetValue = moveDirection.magnitude * moveSpeed;

        // Animation aktualisieren
        if (animationDampingCoroutine == null)
        {
            animationDampingCoroutine = StartCoroutine(UpdateSpeedValueInAnimator());
        }

        // Bewegung ausführen
        Move();
    }

    private IEnumerator UpdateSpeedValueInAnimator()
    {
        float currentValue = animator.GetFloat("Speed");

        while (!Mathf.Approximately(currentValue, targetValue))
        {
            // Geschwindigkeit schrittweise anpassen
            if (currentValue < targetValue)
            {
                currentValue += animationDamping * Time.deltaTime;
                currentValue = Mathf.Min(currentValue, targetValue);
            }
            else
            {
                currentValue -= animationDamping * Time.deltaTime;
                currentValue = Mathf.Max(currentValue, targetValue);
            }

            animator.SetFloat("Speed", currentValue);

            yield return null; // Auf den nächsten Frame warten
        }

        animationDampingCoroutine = null; // Coroutine zurücksetzen
    }

    public void Move()
    {
        CheckIfGrounded();
        ApplyGravity();

        if (moveDirection != Vector3.zero)
        {
            RotateToMoveDirection();
        }

        ApplyMovement();
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ApplyMovement()
    {
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    void RotateToMoveDirection()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotationSpeed);
    }
}
