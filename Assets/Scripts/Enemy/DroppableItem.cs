using UnityEngine;


namespace Enemy
{
    public class DroppableItem : MonoBehaviour
    {

        public DropType dropItemType;
        public GameObject weaponPrefab;
        public enum DropType
        {
            Money,
            Score,
            Nuke,
            Shield,
            Berserker,
            Weapon
        }
        
    }
}