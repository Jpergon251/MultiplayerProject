using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PlayerScripts
{
    public class PlayerControllerGame : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameObject shootingPoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Slider healthSlider;

        [Header("Movimiento del Jugador")]
        [SerializeField] private float playerSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Disparo")]
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float fireRate = 0.25f;
        [SerializeField] private float damage = 20f;

        [Header("Salud y Regeneraci\u00f3n")]
        [SerializeField] private float regenDelay = 3f;
        [SerializeField] private float regenTickInterval = 0.5f;
        [SerializeField] private float healthToRegen = 25f;

        [Header("Vida del Jugador")]
        [Range(0f, 1000f)] public float playerCurrentHealth;
        public float playerMaxHealth = 1000f;

        // Variables internas
        private float regenCooldownTimer;
        private float regenTickTimer;
        private bool isTakingDamage = false;

        private float shootTimer;
        private Vector2 moveInput;
        private bool isShooting = false;

        private CinemachineCamera playerCamera;
        private Camera mainCamera;
        private Rigidbody rb;

        private void Awake()
        {
            playerCamera = GameObject.Find("PlayerCamera")?.GetComponent<CinemachineCamera>();
            mainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();

            if (!shootingPoint || !bulletPrefab)
                Debug.LogError("Faltan referencias asignadas en el inspector.");

            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        private void Start()
        {
            playerCurrentHealth = playerMaxHealth;
        }

        private void Update()
        {
            LookToMouse();       // Girar al jugador hacia el rat\u00f3n
            HandleShooting();    // Controlar disparo
            HandleHealthRegen(); // Regenerar vida si es posible
            UpdateHealthBar();   // Refrescar la barra de vida
        }

        private void FixedUpdate()
        {
            MovePlayerPhysics(); // Movimiento basado en fuerzas (mejor para colisiones)
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void MovePlayerPhysics()
        {
            Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

            // Dirección basada en la cámara
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0f; // Aseguramos que la componente Y no afecte la dirección de la cámara
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = (camForward * input.z + camRight * input.x).normalized;

            // Realizamos un raycast en la dirección del movimiento para detectar colisiones
            RaycastHit hit;
            float checkDistance = 1f; // Distancia para la detección

            if (Physics.Raycast(transform.position, moveDirection, out hit, checkDistance))
            {
                // Si hay una colisión con la pared o rampa, calculamos la normal
                Vector3 normal = hit.normal;

                // Proyectamos la dirección del movimiento sobre el plano de la superficie
                Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, normal);

                // Aplicamos el movimiento sin modificar la Y
                rb.linearVelocity =
                    slideDirection * playerSpeed +
                    new Vector3(0, rb.linearVelocity.y, 0); // Mantener la Y para la gravedad

                // Si el ángulo de la rampa es muy inclinado (por ejemplo, mayor que 45 grados), evitamos el salto
                if (Vector3.Angle(normal, Vector3.up) > 45f)
                {
                    // Si es muy inclinado, limitamos la velocidad en Y para evitar el "salto"
                    rb.linearVelocity =
                        new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Evitar el salto inesperado
                }
            }
            else
            {
                // Si no hay colisión, aplicamos el movimiento normal
                rb.linearVelocity = moveDirection * playerSpeed + new Vector3(0, rb.linearVelocity.y, 0);
            }

            // Si no hay input, frenamos suavemente
            if (moveDirection == Vector3.zero)
            {
                Vector3 velocity = rb.linearVelocity;
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.fixedDeltaTime * 5f);
                velocity.z = Mathf.Lerp(velocity.z, 0, Time.fixedDeltaTime * 5f);
                rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
            }
        }

        public void LookToMouse()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetDirection = hit.point - transform.position;
                targetDirection.y = 0f;

                if (targetDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }

        public void StartShooting(InputAction.CallbackContext context)
        {
            if (context.performed) isShooting = true;
            else if (context.canceled) isShooting = false;
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

        private void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.transform.position, shootingPoint.transform.rotation);
            BulletController bulletScript = bullet.GetComponent<BulletController>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.SetMoveDirection(shootingPoint.transform.forward, bulletSpeed);
            }
        }

        private void HandleHealthRegen()
        {
            if (playerCurrentHealth >= playerMaxHealth) return;

            if (isTakingDamage)
            {
                regenCooldownTimer -= Time.deltaTime;
                if (regenCooldownTimer <= 0f)
                {
                    isTakingDamage = false;
                    regenTickTimer = 0f;
                }
            }
            else
            {
                regenTickTimer -= Time.deltaTime;
                if (regenTickTimer <= 0f)
                {
                    playerCurrentHealth += healthToRegen;
                    playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth);
                    regenTickTimer = regenTickInterval;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            playerCurrentHealth -= damage;
            regenCooldownTimer = regenDelay;
            isTakingDamage = true;

            if (playerCurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Jugador ha muerto.");
            // Aquí podrías notificar al GameManager, jugar animaciones, etc.
        }

        private void UpdateHealthBar()
        {
            if (healthSlider != null)
            {
                healthSlider.value = playerCurrentHealth;
            }
        }
    }
}