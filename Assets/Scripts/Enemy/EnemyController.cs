using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHealth; // Salud máxima del enemigo
    private float currentHealth; // Salud actual del enemigo
    public float moveSpeed = 3f; // Velocidad de movimiento del enemigo
    private Transform player; // Referencia al jugador
    private bool isDead = false;

    public event System.Action OnDeath; // Evento para cuando el enemigo muere

    private void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!isDead)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f && !isDead) // Asegurarse de que no muera más de una vez
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Si ya está muerto, no hacer nada más
        isDead = true; // Marcamos al enemigo como muerto
        OnDeath?.Invoke(); // Invocamos el evento de muerte
        SpawnManager.Instance.HandleEnemyDeath(); // Notificamos al SpawnManager de la muerte
        Destroy(gameObject); // Destruimos el objeto enemigo
    }
}

