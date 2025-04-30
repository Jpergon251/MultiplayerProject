using UnityEngine;

namespace UsableScripts
{
    public class HideMeshRender : MonoBehaviour
    {

        private MeshRenderer _meshRenderer;
        void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = false;
        }

    
    }
}
