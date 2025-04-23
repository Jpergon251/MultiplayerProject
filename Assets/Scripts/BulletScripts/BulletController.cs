using Enemy;
using UnityEngine;

namespace BulletScripts
{
    public class BulletController : MonoBehaviour
    {
        public float bulletLife = 1f;
        public float damage;
        public float speed;
        private Vector3 _direction;

        private void Awake()
        {
            // Destruir la bala después de 5 segundos
            Destroy(gameObject, bulletLife);
        }

        // Método para establecer la dirección de movimiento y la velocidad de la bala
        public void SetMoveDirection(Vector3 moveDirection, float bulletSpeed)
        {
            _direction = moveDirection;  // Dirección en la que se moverá la bala
            speed = bulletSpeed;        // Velocidad de la bala
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyController enemy = other.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);

                    // 💥 Dar puntos por impacto
                    _shooter?.AddScore(10);
                }

                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Mover la bala en la dirección establecida
            transform.Translate(_direction * (speed * Time.deltaTime), Space.World);
        }
        
        private PlayerInventory _shooter;

        public void SetShooter(PlayerInventory player)
        {
            _shooter = player;
        }
    }
}