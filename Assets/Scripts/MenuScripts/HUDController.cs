using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace MenuScripts
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private GameObject arrowPointerPrefab;
        [SerializeField] private Transform arrowPointerContainer;
        [SerializeField] private Transform exitsParent; // ⬅️ El objeto que contiene las salidas

        private List<GameObject> activePointers = new();

        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        public void UpdateExitArrows()
        {
            if (gameManager.inGameMenu.activeSelf)
            {
                ClearAllArrows();

                foreach (Transform exit in exitsParent)
                {
                    var exitComponent = exit.GetComponent<Exits.ExitsController>();
                    if (exitComponent && exitComponent.isActive)
                    {
                        GameObject pointer = Instantiate(arrowPointerPrefab, arrowPointerContainer);
                        pointer.SetActive(true);

                        var tracker = pointer.GetComponent<ArrowTracker>();
                        if (tracker)
                        {
                            tracker.target = exit.transform;
                        }

                        activePointers.Add(pointer);
                    }
                }
            }
        }

        public void ClearAllArrows()
        {
            foreach (var pointer in activePointers)
            {
                Destroy(pointer);
            }
            activePointers.Clear();
        }
    }
}