using System;
using Exits;
using UnityEngine;

namespace Managers
{
    public class ExitsManager : MonoBehaviour
    {
        
        public static ExitsManager Instance;

        [Header("Exits Manager")]
        public ExitsController[] exits;
        public DirectionType lastExitUsed = DirectionType.None;
        
        public enum DirectionType
        {
            North,
            East,
            South,
            West,
            None
        }

        private void Start()
        {
            exits = FindObjectsByType<ExitsController>(FindObjectsSortMode.None);
            lastExitUsed = DirectionType.None;
            DeactivateAllExits();
        }

        // === Unity Methods ===
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

        // === Public Methods ===

        // Activa todas las salidas
        public void ActivateAllExits()
        {
            foreach (var exit in exits)
            {
                exit.ActivateExit();
            }
        }

        // Desactiva todas las salidas
        public void DeactivateAllExits()
        {
            foreach (var exit in exits)
            {
                exit.DeactivateExit();
            }
        }

        public void ActivateAllExitsExceptOpposite()
        {
            var opposite = GetOppositeDirection(lastExitUsed);

            foreach (var exit in exits)
            {
                if (exit.exitDirection != opposite)
                {
                    exit.ActivateExit();
                }
            }
        }

        public DirectionType GetOppositeDirection(DirectionType dir)
        {
            return dir switch
            {
                DirectionType.North => DirectionType.South,
                DirectionType.South => DirectionType.North,
                DirectionType.East => DirectionType.West,
                DirectionType.West => DirectionType.East,
                _ => DirectionType.None
            };
        }
    }
}