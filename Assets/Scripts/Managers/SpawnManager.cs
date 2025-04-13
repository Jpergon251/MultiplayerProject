using System;
using Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        [Header("Enemy Spawning")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnInterval = 3f;

        private float _timer;


        private void Start()
        {
            LoadSpawnPoints();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            
        }

        private void Update()
        {
            // Asegúrate de que no se spawneen más enemigos si ya se alcanzó el máximo
            if (GameManager.Instance.currentEnemiesAlive >= GameManager.Instance.enemiesPerRound)
            {
                return; // No hacer nada si ya hemos alcanzado el máximo de enemigos
            }

            _timer += Time.deltaTime;

            if (_timer >= spawnInterval && GameManager.Instance.currentEnemiesAlive < GameManager.Instance.enemiesPerRound)
            {
                SpawnEnemy();
                _timer = 0f;
            }
        }

        private void LoadSpawnPoints()
        {
            Debug.Log("Cargados los SpawnPoints");
            GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnEnemigo");
            spawnPoints = new Transform[spawnObjects.Length];

            for (int i = 0; i < spawnObjects.Length; i++)
            {
                spawnPoints[i] = spawnObjects[i].transform;
            }
        }
        private void SpawnEnemy()
        {
            if (!GameManager.Instance.roundEnded && GameManager.Instance.enemiesSpawned < GameManager.Instance.enemiesPerRound)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform spawn = spawnPoints[randomIndex];

                // Instanciamos el enemigo
                GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
                GameManager.Instance.currentEnemiesAlive++;
                GameManager.Instance.enemiesSpawned++;
                // Asignar salud extra a cada enemigo dependiendo de la ronda
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.maxHealth += (GameManager.Instance.currentRound - 1) * GameManager.Instance.enemyHealthIncreasePerRound; // Aumento de salud por ronda
                }
            }
        
        }

        public void HandleEnemyDeath()
        {
            if (GameManager.Instance.currentEnemiesAlive > 0) // Verificar que no estemos decreciendo cuando ya no hay enemigos vivos
            {
                GameManager.Instance.currentEnemiesAlive--;
                GameManager.Instance.EnemyDied();
            }
        }
    }
}
