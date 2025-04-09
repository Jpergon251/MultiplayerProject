using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerScripts
{
    public class PlayerControllerGame : MonoBehaviour
    {
        [Header("Player Settings")] 
        [SerializeField] private GameObject shootingPoint;
        [SerializeField] private GameObject bulletPrefab;
        public float playerSpeed = 5f;
        public float rotationSpeed = 10f;
        public float bulletSpeed = 10f;
        public float fireRate = 0.25f;
        public float damage = 20f;
        
        private float shootTimer;
        private Vector2 moveInput;
        private bool isShooting = false;
        private CinemachineCamera playerCamera; // Usamos CinemachineVirtualCamera
        private Camera mainCamera; // La cámara principal controlada por Cinemachine

        
        private void Awake()
        {
            playerCamera = GameObject.Find("PlayerCamera")?.GetComponent<CinemachineCamera>();
            mainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();

            if (!shootingPoint || !bulletPrefab)
                Debug.LogError("Faltan referencias asignadas en el inspector.");
        }

        public void LookToMouse()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetDirection = hit.point - transform.position;
                targetDirection.y = 0f; // Mantener rotación solo en el eje Y

                if (targetDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation =
                        Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }

        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        private void MovePlayer()
        {
            // Convertir el input 2D a dirección en el plano XZ
            Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

            // Obtener los vectores forward y right de la cámara
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            // Eliminar inclinación vertical
            camForward.y = 0f;
            camRight.y = 0f;

            // Normalizar para evitar escalado raro
            camForward.Normalize();
            camRight.Normalize();

            // Dirección final del movimiento
            Vector3 moveDirection = (camForward * input.z + camRight * input.x).normalized;

            // Mover al personaje
            transform.position += moveDirection * (playerSpeed * Time.deltaTime);
        }

        public void StartShooting(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Activar disparo continuo
                isShooting = true;
            }
            else if (context.canceled)
            {
                // Detener disparo cuando se suelta el botón
                isShooting = false;
            }
        }
        private void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.transform.position, shootingPoint.transform.rotation);

            BulletController bulletScript = bullet.GetComponent<BulletController>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.SetMoveDirection(shootingPoint.transform.forward, bulletSpeed);            }
        }
        private void HandleShooting()
        {
            shootTimer += Time.deltaTime;
            if (isShooting && shootTimer >= fireRate)
            {
                Shoot();
                shootTimer = 0f;
            }
        }
        private void Update()
        {
            LookToMouse();
            MovePlayer();
            HandleShooting();
        }
    }
}