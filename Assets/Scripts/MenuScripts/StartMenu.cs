using UnityEngine;

namespace MenuScripts
{
    public class StartMenu : MonoBehaviour
    {
       public GameObject keyToPressText;

        public void FlickKeyToPress()
        {
            keyToPressText.SetActive(!keyToPressText.activeSelf);
        }
    }
}
