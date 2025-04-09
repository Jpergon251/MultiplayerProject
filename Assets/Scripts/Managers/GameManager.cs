using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Game Settings")]
    public int currentRound = 1;
    public int enemiesPerRound = 10; // Número de enemigos por ronda
    public float roundDelay = 3f; // Tiempo de espera entre rondas
    public float enemyHealthIncreasePerRound = 20f; // Aumento de salud por ronda
    public int currentEnemiesAlive;



    public int enemiesSpawned;
    
    public bool roundStarted = false;
    public bool roundEnded = false;
    private int enemiesKilled = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        roundStarted = true;
    }

    private void Update()
    {
        
    }

    public void EnemyDied()
    {
        enemiesKilled++;
        if (enemiesKilled >= enemiesPerRound) // Si se mataron todos los enemigos
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        roundEnded = true;
        roundStarted = false;
        Debug.Log("Ronda " + currentRound + " terminada.");
    }

    private void StartNextRound()
    {
        currentRound++;
        enemiesKilled = 0; // Reiniciar el contador de muertes
        enemiesPerRound += 5; // Puedes aumentar los enemigos por ronda, si lo deseas
        enemyHealthIncreasePerRound += 10f; // Aumentar salud de los enemigos, si lo deseas

        roundStarted = true;
        roundEnded = false;
        currentEnemiesAlive = 0;
    }
}
