using System;
using UnityEngine;

namespace MenuScripts
{
    public class GeneralSettingsManager : MonoBehaviour
    {
        private static GeneralSettingsManager _instance;
        
        
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
