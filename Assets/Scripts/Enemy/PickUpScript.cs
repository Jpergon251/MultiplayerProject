using UnityEngine;
using Weapons;

namespace Enemy
{
    public class PickUpScript : MonoBehaviour
    {
        
        private DroppableItem _droppableItem;
        private void Awake()
        {
            _droppableItem = gameObject.GetComponent<DroppableItem>();
            
            Destroy(gameObject, 20f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerInventory player = other.GetComponent<PlayerInventory>();

                if (player != null)
                {
                    // Obtener el componente DroppableItem del objeto que ha colisionado
                    

                    if (_droppableItem != null)
                    {
                        if (_droppableItem.dropItemType == DroppableItem.DropType.Weapon)
                        {
                            WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();

                            
                            if (weaponHandler && _droppableItem.weaponPrefab != null)
                            {
                                weaponHandler.EquipNewWeapon(_droppableItem.weaponPrefab);
                            }
                        }
                        else
                        {
                            ApplyDropEffect(player);   
                        }
                    }
                }

                Destroy(gameObject); // Se destruye al recogerse
            }
        }

        private void ApplyDropEffect(PlayerInventory player)
        {
            switch (_droppableItem.dropItemType)
            {
                case DroppableItem.DropType.Money:
                    player.AddMoney(50); // Puedes también parametrizar la cantidad en DropItemData si quieres
                    break;
                case DroppableItem.DropType.Score:
                    player.AddScore(100);
                    break;
                case DroppableItem.DropType.Nuke:
                    player.AddNuke(1); // Este método lo defines tú en PlayerControllerGame
                    break;
                case DroppableItem.DropType.Shield:
                    player.AddShield(1); // Lo preparas para futuro
                    break;
                case DroppableItem.DropType.Berserker:
                    player.AddBerserker(1); // También futuro
                    break;
            }
        }
    }
}
