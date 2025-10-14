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

    void Update() { }

    public void Move(InputAction.CallbackContext context)
    {
        if (isCrouching)
            return;

        moveInput = context.ReadValue<Vector2>();
        horizontalMove = moveInput.x * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
        }
        else if (context.canceled)
        {
            isCrouching = false;
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
        moveInput = Vector2.zero;
        animator.SetBool("IsCrouching", isCrouching);
    }
}
