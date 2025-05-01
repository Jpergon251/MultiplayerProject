using Managers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Weapons;

namespace PlayerScripts
{
    public class PlayerControllerGame : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameObject shootingPoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private GameObject inGameMenu;
        [SerializeField] private GameObject gameOverMenu;
       

        [Header("Movimiento del Jugador")]
        [SerializeField] private float playerSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        

        [Header("Salud y Regeneracion")]
        [SerializeField] private float regenDelay = 3f;
        [SerializeField] private float regenTickInterval = 0.5f;
        [SerializeField] private float healthToRegen = 25f;

        [Header("Vida del Jugador")]
        [Range(0f, 1000f)] public float playerCurrentHealth;
        public float playerMaxHealth = 1000f;

        [Header("Arma")]
        private WeaponHandler _weaponHandler;
        
        // Variables internas
        private float _regenCooldownTimer;
        private float _regenTickTimer;
        private float _shootTimer;
        
        private bool _isShooting;
        private bool _isTakingDamage;
        public bool isDead;
        
        private Vector2 _moveInput;
        
        private Camera _mainCamera;
        private Rigidbody _rb;
        private PlayerInventory _inventory;

        private void Awake()
        {
            _mainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();
            
            if (!shootingPoint || !bulletPrefab)
                Debug.LogError("Faltan referencias asignadas en el inspector.");

            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _inventory = GetComponent<PlayerInventory>(); // Añadir el componente Inventory
            _weaponHandler = GetComponent<WeaponHandler>();
        }

        private void Start()
        {
            playerCurrentHealth = playerMaxHealth;
            isDead = false;
        }

        private void Update()
        {
            LookToMouse();       // Girar al jugador hacia el ratón
            HandleShooting();    // Controlar disparo
            HandleHealthRegen(); // Regenerar vida si es posible
        }

        private void FixedUpdate()
        {
            MovePlayerPhysics(); // Movimiento basado en fuerzas (mejor para colisiones)
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void MovePlayerPhysics()
        {
            if (!isDead)
            {
                Vector3 input = new Vector3(_moveInput.x, 0f, _moveInput.y);

                // Dirección basada en la cámara
                Vector3 camForward = _mainCamera.transform.forward;
                Vector3 camRight = _mainCamera.transform.right;
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
                    _rb.linearVelocity =
                        slideDirection * playerSpeed +
                        new Vector3(0, _rb.linearVelocity.y, 0); // Mantener la Y para la gravedad

                    // Si el ángulo de la rampa es muy inclinado (por ejemplo, mayor que 45 grados), evitamos el salto
                    if (Vector3.Angle(normal, Vector3.up) > 45f)
                    {
                        // Si es muy inclinado, limitamos la velocidad en Y para evitar el "salto"
                        _rb.linearVelocity =
                            new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z); // Evitar el salto inesperado
                    }
                }
                else
                {
                    // Si no hay colisión, aplicamos el movimiento normal
                    _rb.linearVelocity = moveDirection * playerSpeed + new Vector3(0, _rb.linearVelocity.y, 0);
                }

                // Si no hay input, frenamos suavemente
                if (moveDirection == Vector3.zero)
                {
                    Vector3 velocity = _rb.linearVelocity;
                    velocity.x = Mathf.Lerp(velocity.x, 0, Time.fixedDeltaTime * 5f);
                    velocity.z = Mathf.Lerp(velocity.z, 0, Time.fixedDeltaTime * 5f);
                    _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);
                }
            }
        }

        public void LookToMouse()
        {
            if (!isDead)
            {
                Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 targetDirection = hit.point - transform.position;
                    targetDirection.y = 0f;

                    if (targetDirection.sqrMagnitude > 0.01f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                            rotationSpeed * Time.deltaTime);
                    }
                }
            }
        }

        public void StartShooting(InputAction.CallbackContext context)
        {
            if(!isDead)
            {
                if (context.performed) _isShooting = true;
                else if (context.canceled) _isShooting = false;
            }
        }

        private void HandleShooting()
        {
            if (_weaponHandler.currentWeaponData == null) return;

            _shootTimer += Time.deltaTime;

            if (_isShooting && _shootTimer >= _weaponHandler.currentWeaponData.fireRate)
            {
                _weaponHandler.Shoot(_inventory);
                _shootTimer = 0f;
            }
        }

        private void HandleHealthRegen()
        {
            if (playerCurrentHealth >= playerMaxHealth) return;

            if (_isTakingDamage)
            {
                _regenCooldownTimer -= Time.deltaTime;
                if (_regenCooldownTimer <= 0f)
                {
                    _isTakingDamage = false;
                    _regenTickTimer = 0f;
                }
            }
            else
            {
                _regenTickTimer -= Time.deltaTime;
                if (_regenTickTimer <= 0f)
                {
                    playerCurrentHealth += healthToRegen;
                    UpdateHealthBar();
                    playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth);
                    _regenTickTimer = regenTickInterval;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            playerCurrentHealth -= damage;
            UpdateHealthBar();
            _regenCooldownTimer = regenDelay;
            _isTakingDamage = true;

            if (playerCurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Jugador ha muerto.");
            isDead = true;
            inGameMenu.SetActive(false);
            gameOverMenu.SetActive(true);
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            gameObject.SetActive(false);
            GameManager.Instance.isPlayerDead = true;
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
