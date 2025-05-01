using UnityEngine;

namespace Animations
{
    public class FadeTransition : MonoBehaviour
    {
        public static FadeTransition Instance;

        private Animator animator;

        private void Awake()
        {
            Instance = this;
            animator = GetComponent<Animator>();
        }

        public void PlayFadeOut()
        {
            animator.Play("FadeOut");
        }

        public void PlayFadeIn()
        {
            animator.Play("FadeIn");
        }
    }
}
