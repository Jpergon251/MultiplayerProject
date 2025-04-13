using System;
using Managers;
using PlayerScripts;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Stats")]
        public float maxHealth = 100f;
        private float _currentHealth;

        [Header("Movimiento")]
        public float moveSpeed = 3f;

        [Header("Ataque")]
        public float attackDamage = 150f;
        public float attackCooldown = 3f;
        private float _attackTimer;
        private bool _hasAttackedOnce;
        // Estados internos
        private bool _isPlayerNear;
        private bool _isDead;

        // Referencias
        private PlayerControllerGame _player;

        // Eventos
        public event Action OnDeath;

        // Trigger: detectar si el jugador está cerca
    
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNear = true;
                _attackTimer += Time.deltaTime;

                // Realizar el primer ataque inmediatamente si no se ha hecho aún
                if (!_hasAttackedOnce)
                {
                    AttackPlayer(attackDamage);
                    _hasAttackedOnce = true; // Marcar que el primer ataque ya se realizó
                    _attackTimer = 0f; // Reiniciar el temporizador
                }

                // Para los ataques siguientes, usar el temporizador con el ataque cooldown
                if (_attackTimer >= attackCooldown)
                {
                    AttackPlayer(attackDamage);
                    _attackTimer = 0f; // Reiniciar el temporizador después de cada ataque
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNear = false;
                _attackTimer = 0f;
                _hasAttackedOnce = false;
            }
        }

        private void Awake()
        {
            _currentHealth = maxHealth;
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerGame>();
        }

        private void Update()
        {
            if (!_isDead)
            {
                MoveTowardsPlayer();
            }
        }

        private void MoveTowardsPlayer()
        {
            if (_player == null) return;

            Vector3 direction = (_player.transform.position - transform.position).normalized;
            transform.position += direction * (moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        private void AttackPlayer(float dmg)
        {
            if (!_isPlayerNear || _player == null) return;

            _player.TakeDamage(dmg);
            Debug.Log("Atacando al jugador");
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0f && !_isDead)
            {
                Die();
            }
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            OnDeath?.Invoke();
            SpawnManager.Instance.HandleEnemyDeath();
            Destroy(gameObject);
        }
    }
}
