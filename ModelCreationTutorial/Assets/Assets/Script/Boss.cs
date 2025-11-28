using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBoss : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject projectilePrefab;
    public GameObject mistakePrefab;
    public GameObject forwardProjectilePrefab;
    public Transform player;
    public Transform projectileSpawnPoint;
    public Transform forwardSpawnPoint;
    public float dropAttackInterval = 3f;
    public float forwardAttackInterval = 5f;
    public float MistakeAttackInterval = 8f;
    public GameObject VictoryText;


    private float dropAttackTimer;
    private float forwardAttackTimer;
    public float MistakeAttackTimer;

    private void Start()
    {
        currentHealth = maxHealth;

        if (VictoryText != null)
        {
            VictoryText.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            HandleGameOverInput();
            return;
        }

        dropAttackTimer += Time.deltaTime;
        if (dropAttackTimer >= dropAttackInterval)
        {
            AttackPlayer();
            dropAttackTimer = 0f;
        }

        forwardAttackTimer += Time.deltaTime;
        if (forwardAttackTimer >= forwardAttackInterval)
        {
            ForwardAttack();
            forwardAttackTimer = 0f;
        }
        MistakeAttackTimer += Time.deltaTime;
        if (MistakeAttackTimer >= MistakeAttackInterval)
        {
            MistakeAttack();
            MistakeAttackTimer = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Mistake"))
        {
            TakeDamage(5);
            Destroy(collision.gameObject);
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
        Time.timeScale = 0.1f;

        if (VictoryText != null)
        {
            VictoryText.SetActive(true);
        }
    }

    private void HandleGameOverInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void AttackPlayer()
    {
        if (player != null && projectileSpawnPoint != null)
        {
            Vector3 spawnPosition = new Vector3(player.position.x, player.position.y + 30f, player.position.z);
            projectileSpawnPoint.position = spawnPosition;

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0, -10f, 0);
            }
        }
    }

    private void ForwardAttack()
    {
        if (forwardSpawnPoint != null)
        {
            Instantiate(forwardProjectilePrefab, forwardSpawnPoint.position, Quaternion.identity);
        }
    }

    private void MistakeAttack()
    {
        if (player != null && forwardSpawnPoint != null)
        {
            Vector3 spawnPosition = new Vector3(player.position.x, player.position.y + 30f, player.position.z);
            projectileSpawnPoint.position = spawnPosition;
            GameObject projectile = Instantiate(mistakePrefab, projectileSpawnPoint.position, Quaternion.identity);
        }
    }
}
