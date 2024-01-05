using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float movementSpeed = 8f;
    public float walkSpeed;
    public float sprintSpeed;
    public float footStepSoundRadius;

    [Header("Jumping")]
    public float jumpHeight = 1f;
    Vector3 velocity;

    [Header("Crouching")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Stamina")]
    public float stamina = 100f;
    public Image staminaBarFill;
    public CanvasGroup staminaBarCG;
    public float tiredDuration = 0f;

    [Header("Physics")]
    public float gravity = -9.81f * 3;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    [Header("Audio")]
	public AudioClip footStepSound;
    public float footStepDelay = 0.35f;
    public float nextFootstep = 0;

    public MovementState movementState;
    public enum MovementState
    {
        walking, sprinting, crouching, air
    }

    Rigidbody rb;

    private void MovementStateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            movementState = MovementState.crouching;
            movementSpeed = crouchSpeed;
            footStepSoundRadius = 5f;
        }
        // Mode - Sprinting
        if (Input.GetKey(sprintKey) && tiredDuration <= 0f)
        {
            movementState = MovementState.sprinting;
            movementSpeed = sprintSpeed;
            footStepSoundRadius = 20f;
        }
        // Mode - Walking
        else if (isGrounded)
        {
            movementState = MovementState.walking;
            movementSpeed = walkSpeed;
            footStepSoundRadius = 10f;
        }
        else
        {
            movementState = MovementState.air;
        }
    }

    void Start()
    {
        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        MovementStateHandler();

        if (movementState == MovementState.sprinting && tiredDuration <= 0f)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
            {
                if (stamina > 0f) stamina -= 50 * Time.deltaTime;
                ShowStaminaBar(true);
            }

            if (stamina <= 0f) tiredDuration = 100f;
        }
        else
        {
            if (stamina < 100f) stamina += 50 * Time.deltaTime;
            if (stamina >= 100f) ShowStaminaBar(false);
            if (tiredDuration > 0f) tiredDuration -= 50 * Time.deltaTime;
        }

        staminaBarFill.fillAmount = stamina / 100f;

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
        

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
		{
            velocity.y = -2f;	
		}
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * movementSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
		{
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) && isGrounded)
		{
			nextFootstep -= Time.deltaTime;
			if (nextFootstep <= 0) 
			{
                ProjectFootstepSphere();
				GetComponent<AudioSource>().PlayOneShot(footStepSound, 0.7f);
				nextFootstep += footStepDelay;
			}
		}
    }

    private void ProjectFootstepSphere()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, footStepSoundRadius);
        foreach (Collider collider in hitColliders)
        { 
            if (collider.gameObject.name == "Enemy")
            {
                EnemyAI enemy = collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.ChasePlayer();
				}
			}
		}
	}

    private void ShowStaminaBar(bool show)
    {
        if (show)
        {
            if (staminaBarCG.alpha < 1) staminaBarCG.alpha += 2 * Time.deltaTime;
        }
        else
        {
            if (staminaBarCG.alpha > 0) staminaBarCG.alpha -= 2 * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Adjust color as needed
        Gizmos.DrawWireSphere(transform.position, footStepSoundRadius);
    }
}
