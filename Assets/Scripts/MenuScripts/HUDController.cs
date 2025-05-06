// Cada puerta tiene un componente tipo ExitController con isActive

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MenuScripts
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private GameObject arrowPointerPrefab;
        [SerializeField] private Transform arrowPointerContainer;
        [SerializeField] private Transform exitsParent; // ⬅️ El objeto que contiene las salidas

        private List<GameObject> activePointers = new();


        private void Start()
        {
            gameObject.SetActive(true);
        }

        public void UpdateExitArrows()
        {
            ClearAllArrows();

            foreach (Transform exit in exitsParent)
            {
                var exitComponent = exit.GetComponent<Exits.ExitsController>();
                if (exitComponent != null && exitComponent.isActive)
                {
                    GameObject pointer = Instantiate(arrowPointerPrefab, arrowPointerContainer);
                    pointer.SetActive(true);

                    var tracker = pointer.GetComponent<ArrowTracker>();
                    if (tracker != null)
                    {
                        tracker.target = exit.transform;
                    }

                    activePointers.Add(pointer);
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