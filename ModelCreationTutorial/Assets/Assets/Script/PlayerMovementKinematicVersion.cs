using UnityEngine;
using UnityEngine.SceneManagement;

public class KinematicPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float airDashMultiplier = 0.5f;
    private bool isDashing = false;

    [Header("Camera Settings")]
    public Transform cameraTransform;

    [Header("Projectile Settings")]
    public GameObject pelletPrefab;
    public Transform pelletSpawnPoint;
    public float pelletSpeed = 10f;

    [Header("Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Death Settings")]
    public GameObject deathImage;

    private CharacterController characterController;
    private Vector3 velocity;
    private Vector3 dashDirection;
    private bool isGrounded;
    private float dashTime;
    private bool isDead = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        if (deathImage != null)
        {
            deathImage.SetActive(false);
        }
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ReloadScene();
            }
            return;
        }

        HandleGroundCheck();

        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
            ApplyGravity();
        }
        else
        {
            HandleDash();
        }

        if (Input.GetButtonDown("Fire3"))
        {
            StartDash();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            ShootPellet();
        }
    }

    private void HandleGroundCheck()
    {
        Vector3 groundCheckPosition = transform.position + characterController.center - new Vector3(0, characterController.height / 2, 0);
        isGrounded = Physics.CheckSphere(groundCheckPosition, groundCheckDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(moveDir * moveSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    private void StartDash()
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTime = dashDuration;
            velocity.y = 0;
            dashDirection = transform.forward;

            if (!isGrounded)
            {
                dashDirection *= airDashMultiplier;
            }
        }
    }

    private void HandleDash()
    {
        if (dashTime > 0)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    private void ShootPellet()
    {
        if (pelletPrefab != null && pelletSpawnPoint != null)
        {
            GameObject pellet = Instantiate(pelletPrefab, pelletSpawnPoint.position, Quaternion.identity);

            Rigidbody pelletRigidbody = pellet.GetComponent<Rigidbody>();
            if (pelletRigidbody != null)
            {
                pelletRigidbody.linearVelocity = transform.forward * pelletSpeed;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyAttack"))
        {
            TakeDamage(1);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (deathImage != null)
        {
            deathImage.SetActive(true);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
