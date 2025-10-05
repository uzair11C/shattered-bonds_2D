using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D controller;

    [SerializeField]
    private Animator animator;

    private PlayerControls controls;
    private Vector2 moveInput;

    private float horizontalMove = 0f;
    private bool isJumping = false;
    private bool isCrouching = false;

    public float runSpeed = 40f;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // moveInput = controls.FindAction("Move").ReadValue<Vector2>();
        // horizontalMove = moveInput.x * runSpeed;

        // Attack
        // if (controls.FindAction("Attack").triggered)
        // {
        //     Debug.Log("Melee Attack");
        // }
        // horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        // animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        // if (Input.GetButtonDown("Jump"))
        // {
        //     isJumping = true;
        //     animator.SetBool("IsJumping", true);
        // }

        // if (Input.GetButtonDown("Crouch"))
        // {
        //     isCrouching = true;
        // }
        // else if (Input.GetButtonUp("Crouch"))
        // {
        //     isCrouching = false;
        // }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isCrouching)
            return;

        moveInput = context.ReadValue<Vector2>();
        horizontalMove = moveInput.x * runSpeed;
        Debug.Log($"Move: {moveInput}");
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
            Debug.Log("Jump");
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            Debug.Log("Crouch started");
        }
        else if (context.canceled)
        {
            isCrouching = false;
            Debug.Log("Crouch ended");
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping);
        isJumping = false;
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }
}
