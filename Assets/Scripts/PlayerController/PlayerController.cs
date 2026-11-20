using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6.0f;
    public float jumpForce = 5.0f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.2f;

    Rigidbody rb;
    [Header("Camera")]
    public float mouseSensitivity = 2.0f;
    public float lookLimit = 80f;


    private KeyCode blockKey = KeyCode.Mouse1; // right mouse
    private KeyCode swordKey = KeyCode.Mouse0; // left mouse

    private bool blockActive;
    private bool swordActive;

    private float blockDuration = 0.8f;
    private float swordDuration = 1f;

    private float blockTimer;
    private float swordTimer;

    private bool isGrounded;
    private bool jumpRequested;

    private float xRotation = 0f;
    private float yRotation = 0f;


    [Header("Enemy Attack Settings")]
    public Animator animator;
    public LayerMask enemyLayer;
    public Collider weapon;
    public int damage = 20;

    public bool BlockActive => blockActive;
    public bool SwordActive => swordActive;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // don't let collisions tip the player over
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
    }

    private void ReadInput()
    {
        blockActive = Input.GetKeyDown(blockKey); // only the press frame
        swordActive = Input.GetKeyDown(swordKey); // only the press frame
    }

    private void HandleCombat()
    {
        if (blockTimer > 0)
        {
            blockTimer -= Time.deltaTime;
            if (blockTimer <= 0)
                animator.SetBool("blockActive", false);
        }

        if (swordTimer > 0)
        {
            swordTimer -= Time.deltaTime;
            if (swordTimer <= 0)
                animator.SetBool("swordActive", false);
        }

        if (blockActive && swordActive)
            return;

        bool busy = blockTimer > 0 || swordTimer > 0;

        if (blockActive && !busy)
        {
            animator.SetBool("blockActive", true);
            blockTimer = blockDuration;
        }

        if (swordActive && !busy)
        {
            animator.SetBool("swordActive", true);
            swordTimer = swordDuration;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 8;

        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -lookLimit, lookLimit);

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Ground Check using a Raycast downwards
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);


        //// Capture jump input in Update so frames aren't missed
        //if (Input.GetButtonDown("Jump") && isGrounded)
        //{
        //    jumpRequested = true;
        //}
        Camera.main.GetComponent<Camera>().fieldOfView = 60f;


        // Animator triggers
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            moveSpeed = 0f;
        } else
        {
            moveSpeed = 6f;
        }
            animator.SetFloat("playerSpeed", moveSpeed);
        ReadInput();
        HandleCombat();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Move relative to facing direction, flattened to the ground
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 targetVelocity = (forward * v + right * h) * moveSpeed;

        //// Apply Jump or keep current vertical velocity
        //if (jumpRequested)
        //{
        //    targetVelocity.y = jumpForce;
        //    jumpRequested = false;
        //}
        //else
        //{
        //    targetVelocity.y = rb.linearVelocity.y; // keep gravity untouched
        //}

        rb.linearVelocity = targetVelocity;
    }

}