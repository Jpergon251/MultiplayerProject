using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animations;
using MenuScripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Enemies settings")]
        public int enemiesPerRound = 10;
        public int currentEnemiesAlive;
        public int enemiesSpawned;
        public float enemyHealthIncreasePerRound = 20f;
        
        [Header("Round settings")]
        public int currentRound = 1;
        public float roundDelay = 3f;
        public bool roundStarted;
        public bool roundEnded;

        [Header("Player settings")]
        public bool isPlayerDead;
        public int enemiesKilled;
        
        [Header("HUD settings")]
        private HUDController _hud;
        
        private AudioSettingsManager _audio;
        private VideoSettingsManager _video;
        private ControlsSettingsManager _controls;
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
            _audio = Resources.FindObjectsOfTypeAll<AudioSettingsManager>().FirstOrDefault();
            _video = Resources.FindObjectsOfTypeAll<VideoSettingsManager>().FirstOrDefault();
            roundStarted = true;
            _hud = FindObjectOfType<HUDController>();
            _audio.Initialize();
            _video.Initialize();
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
            
            _hud.UpdateExitArrows();
        }

        private IEnumerator StartNextRoundCoroutine()
        {
            yield return new WaitForSeconds(roundDelay); // Espera opcional de 3 segundos

            currentRound++;
            enemiesKilled = 0;
            enemiesPerRound += 5;

            roundStarted = true;
            roundEnded = false;
            currentEnemiesAlive = 0;
            enemiesSpawned = 0;

            // Aquí podrías poner una animación de "¡Nueva Ronda!", un sonido, etc.
            Debug.Log("Comienza la ronda " + currentRound);
        }
        
        public void HandleExitUsed(ExitsManager.DirectionType usedExit)
        {
            
            StartCoroutine(HandleExitUsedCoroutine(usedExit));
            
        }

        private IEnumerator HandleExitUsedCoroutine(ExitsManager.DirectionType usedExit)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            CinemachineOrbitalFollow orbitalFollow = GameObject.Find("PlayerCamera").GetComponent<CinemachineOrbitalFollow>();

            
            FadeTransition.Instance.PlayFadeOut();

            ExitsManager.Instance.DeactivateAllExits();

            
            
            playerInput.enabled = false;
            
            yield return new WaitForSeconds(1f);

            _hud.ClearAllArrows();
            // Debug.Log(orbitalFollow.TrackerSettings.PositionDamping);
            orbitalFollow.TrackerSettings.PositionDamping = Vector3.zero;
            // Debug.Log(orbitalFollow.TrackerSettings.PositionDamping);
            
            // Determina la salida opuesta
            var opposite = ExitsManager.Instance.GetOppositeDirection(usedExit);
            var oppositeExit = ExitsManager.Instance.exits.FirstOrDefault(e => e.exitDirection == opposite);
           
            if (oppositeExit != null)
            {
                player.transform.position = oppositeExit.transform.position;
                player.transform.rotation = oppositeExit.transform.rotation;
            }
            
            yield return new WaitForSeconds(1f); // opcional, si quieres dejar respirar la transición
            
            orbitalFollow.TrackerSettings.PositionDamping = new Vector3(5,1,5);
            // Debug.Log(orbitalFollow.TrackerSettings.PositionDamping);
            playerInput.enabled = true;
            FadeTransition.Instance.PlayFadeIn();
            StartCoroutine(StartNextRoundCoroutine());
        }
    }
}
