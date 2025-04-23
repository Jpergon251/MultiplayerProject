using System.Linq;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game Settings")]
        public int currentRound = 1;
        public int enemiesPerRound = 10;
        public float roundDelay = 3f;
        public float enemyHealthIncreasePerRound = 20f;

        public int currentEnemiesAlive;
        public int enemiesSpawned;

        public bool roundStarted;
        public bool roundEnded;

        public bool isPlayerDead;
        
        public int enemiesKilled;
        
        
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

        private void Start()
        {
            roundStarted = true;
        }

        public void EnemyDied()
        {
            enemiesKilled++;
            if (enemiesKilled >= enemiesPerRound)
            {
                EndRound();
            }
        }

        private void EndRound()
        {
            Debug.Log("Ronda " + currentRound + " terminada.");
            roundEnded = true;
            roundStarted = false;

            if (ExitsManager.Instance.lastExitUsed == ExitsManager.DirectionType.None)
            {
                ExitsManager.Instance.ActivateAllExits();
            }
            else
            {
                ExitsManager.Instance.ActivateAllExitsExceptOpposite();
            }
        }

        private void StartNextRound()
        {
            currentRound++;
            enemiesKilled = 0;
            enemiesPerRound += 5;

            roundStarted = true;
            roundEnded = false;
            currentEnemiesAlive = 0;
            enemiesSpawned = 0;
            ExitsManager.Instance.DeactivateAllExits();
        }
        
        
        public void HandleExitUsed(ExitsManager.DirectionType usedExit)
        {
            

            // Determina la salida opuesta
            var opposite = ExitsManager.Instance.GetOppositeDirection(usedExit);

            // Encuentra el controlador de la salida opuesta
            var oppositeExit = ExitsManager.Instance.exits.FirstOrDefault(e => e.exitDirection == opposite);

            if (oppositeExit != null)
            {
                // Mueve al jugador
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = oppositeExit.transform.position;
                player.transform.rotation = oppositeExit.transform.rotation;

                // Desactiva temporalmente el trigger de entrada
                // oppositeExit.GetComponent<Collider>().enabled = false;


            }

            StartNextRound();
        }
        



    }
}
