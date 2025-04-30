using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private GameObject weaponPrefab; // ← sigue siendo el prefab base, no lo toques

        public GameObject currentWeaponInstance;
        public Weapon currentWeaponData;

        private void Start()
        {
            EquipWeapon();
        }

        public void EquipWeapon()
        {
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

        public Weapon GetCurrentWeaponData() => currentWeaponData;
    }
}