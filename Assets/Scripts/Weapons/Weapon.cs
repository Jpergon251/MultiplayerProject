using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
    public class Weapon : ScriptableObject
    {
        public string weaponName;
        public GameObject bulletPrefab;
        public float fireRate = 0.25f;
        public float bulletSpeed = 150f;
        public float damage = 20f;
    }
}