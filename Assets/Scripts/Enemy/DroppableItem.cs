using UnityEngine;

namespace Enemy
{
    public class DroppableItem : MonoBehaviour
    {

        public DropType dropItemType;
        
        public enum DropType
        {
            Money,
            Score,
            Nuke,
            Shield,
            Berserker
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerInventory player = other.GetComponent<PlayerInventory>();

                if (player != null)
                {
                    ApplyDropEffect(player);
                }

                Destroy(gameObject); // Se destruye al recogerse
            }
        }

        private void ApplyDropEffect(PlayerInventory player)
        {
            switch (dropItemType)
            {
                case DropType.Money:
                    player.AddMoney(50); // Puedes también parametrizar la cantidad en DropItemData si quieres
                    break;
                case DropType.Score:
                    player.AddScore(100);
                    break;
                case DropType.Nuke:
                    player.AddNuke(1); // Este método lo defines tú en PlayerControllerGame
                    break;
                case DropType.Shield:
                    player.AddShield(1); // Lo preparas para futuro
                    break;
                case DropType.Berserker:
                    player.AddBerserker(1); // También futuro
                    break;
            }
        }
    }
}