using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
    public class Weapon : ScriptableObject
    {
        public string weaponName;
        public float fireRate = 0.25f;
        public float damage = 20f;
        public float range = 100f;
        public float penetration = 1f;
        public float dispersion = 1.5f;
    }
}