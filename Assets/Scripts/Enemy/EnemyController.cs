using System;
using PlayerScripts;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Movimiento")]
    public float moveSpeed = 3f;

    [Header("Ataque")]
    public float attackDamage = 150f;
    public float attackCooldown = 3f;
    private float attackTimer = 0f;
    private bool hasAttackedOnce = false;
    // Estados internos
    private bool isPlayerNear = false;
    private bool isDead = false;

    // Referencias
    private PlayerControllerGame player;

    // Eventos
    public event Action OnDeath;

    // Trigger: detectar si el jugador está cerca
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            attackTimer += Time.deltaTime;

            // Realizar el primer ataque inmediatamente si no se ha hecho aún
            if (!hasAttackedOnce)
            {
                AttackPlayer(attackDamage);
                hasAttackedOnce = true; // Marcar que el primer ataque ya se realizó
                attackTimer = 0f; // Reiniciar el temporizador
            }

            // Para los ataques siguientes, usar el temporizador con el ataque cooldown
            if (attackTimer >= attackCooldown)
            {
                AttackPlayer(attackDamage);
                attackTimer = 0f; // Reiniciar el temporizador después de cada ataque
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            attackTimer = 0f;
            hasAttackedOnce = false;
        }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerGame>();
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

        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void AttackPlayer(float dmg)
    {
        if (!isPlayerNear || player == null) return;

        player.TakeDamage(dmg);
        Debug.Log("Atacando al jugador");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
        SpawnManager.Instance.HandleEnemyDeath();
        Destroy(gameObject);
    }
}
