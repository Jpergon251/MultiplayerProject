using System;
using Enemy;
using UnityEngine;

namespace Weapons
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private GameObject weaponPrefab; // ← sigue siendo el prefab base, no lo toques
        [SerializeField] private GameObject bulletTrailPrefab; // Asignar en inspector

        
        public GameObject currentWeaponInstance;
        public Weapon currentWeaponData;

        private void Start()
        {
            EquipWeapon();
        }

        public void EquipNewWeapon(GameObject newWeaponPrefab)
        {
            // 1. Destruir arma anterior si existe
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance = null;
                currentWeaponData = null;
                weaponPrefab = null;
            }
                
            
            // 2. Instanciar nueva arma como hijo del holder
            weaponPrefab = newWeaponPrefab;
            EquipWeapon();
        }
        public void EquipWeapon()
        {
            if (weaponPrefab == null) return;
            // Destruye el arma anterior si existe
            if (currentWeaponInstance != null)
                Destroy(currentWeaponInstance);

            // Instancia la nueva arma como hijo del holder
            currentWeaponInstance = Instantiate(weaponPrefab, weaponHolder);

            // Obtiene el componente WeaponComponent del arma instanciada
            WeaponComponent weaponComponent = currentWeaponInstance.GetComponent<WeaponComponent>();
            if (weaponComponent != null)
            {
                currentWeaponData = weaponComponent.weaponData;
            }
            else
            {
                Debug.LogError("El arma instanciada no tiene WeaponComponent.");
            }
        }

        public void Shoot(PlayerInventory inventory)
        {
            if (!currentWeaponInstance || !currentWeaponData || !weaponPrefab) return;

            WeaponComponent weaponComponent = currentWeaponInstance.GetComponent<WeaponComponent>();
            if (!weaponComponent || !weaponComponent.shootPoint)
            {
                Debug.LogWarning("El arma no tiene WeaponComponent o shootPoint.");
                return;
            }

            Transform shootPoint = weaponComponent.shootPoint;
            Vector3 direction = shootPoint.forward;
            float range = currentWeaponData.range;
            float penetration = currentWeaponData.penetration;
            float baseDamage = currentWeaponData.damage;
            float dispersion = currentWeaponData.dispersion; // entre 0 y 2
            float maxAngle = dispersion * 5f; // Escalado de ángulo (ajustable)
            float angle = UnityEngine.Random.Range(-1f, 1f);
            angle = Mathf.Sign(angle) * Mathf.Pow(Mathf.Abs(angle), 2f); // curva cuadrática

            float finalAngle = angle * maxAngle;
            // Debug.Log("Dispersion:" + dispersion + "\n Angulo: " + angle + "\n Angulo maximo: " + maxAngle + "\n Angulo final:" + finalAngle );
            Vector3 spreadDirection = Quaternion.Euler(0, finalAngle, 0) * direction;
            
            
            RaycastHit[] hits = Physics.RaycastAll(shootPoint.position, spreadDirection, range);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            int fullHits = Mathf.FloorToInt(penetration);
            float partialHitFraction = penetration - fullHits;
            int hitCount = 0;

            Vector3 endPoint = shootPoint.position + spreadDirection * range;

            foreach (RaycastHit hit in hits)
            {
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy)
                {
                    if (hitCount < fullHits)
                    {
                        enemy.TakeDamage(baseDamage);
                        hitCount++;
                        inventory.AddScore(10);
                    }
                    else if (hitCount == fullHits && partialHitFraction > 0f)
                    {
                        enemy.TakeDamage(baseDamage * partialHitFraction);
                        hitCount++;
                        endPoint = hit.point;
                        break;
                    }

                    endPoint = hit.point;
                }
            }

            CreateBulletTrail(shootPoint.position, endPoint); // Podrías mover esto también al WeaponComponent

            Debug.DrawRay(shootPoint.position, spreadDirection * range, Color.red, 1f);
        }
        private void CreateBulletTrail(Vector3 start, Vector3 end)
        {
            GameObject trail = Instantiate(bulletTrailPrefab);
            LineRenderer lr = trail.GetComponent<LineRenderer>();
            if (lr)
            {
                lr.SetPosition(0, start);
                lr.SetPosition(1, end);
            }

            Destroy(trail, 0.02f); // Dura muy poco para parecer un "trazo de bala"
        }
        public Weapon GetCurrentWeaponData() => currentWeaponData;
    }
}