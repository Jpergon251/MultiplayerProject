using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletLife = 1f;
    public float damage;
    public float speed;
    private Vector3 direction;

    private void Awake()
    {
        // Destruir la bala después de 5 segundos
        Destroy(gameObject, bulletLife);
    }

    // Método para establecer la dirección de movimiento y la velocidad de la bala
    public void SetMoveDirection(Vector3 moveDirection, float bulletSpeed)
    {
        direction = moveDirection;  // Dirección en la que se moverá la bala
        speed = bulletSpeed;        // Velocidad de la bala
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>()?.TakeDamage(damage);
            Destroy(gameObject); // Si la bala se destruye al impactar
        }
    }

    private void Update()
    {
        // Mover la bala en la dirección establecida
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
    }
}