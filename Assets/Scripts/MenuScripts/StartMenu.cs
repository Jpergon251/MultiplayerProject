using UnityEngine;

namespace MenuScripts
{
    public class StartMenu : MonoBehaviour
    {
        public GameObject insertCoinText;
        public GameObject keyToPressText;

        private void Start()
        {
            InvokeRepeating("FlickKeyToPress", 0f, 0.5f);
        }

        private void FlickKeyToPress()
        {
            keyToPressText.SetActive(!keyToPressText.activeSelf);
        }
    }
}
