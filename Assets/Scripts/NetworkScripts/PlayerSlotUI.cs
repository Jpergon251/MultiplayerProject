using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace NetworkScripts
{
    public class PlayerSlotUI : NetworkBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;

        public void SetPlayerName(string name)
        {
            playerNameText.text = name;
        }

        public void SetWaiting()
        {
            playerNameText.text = "Waiting for player...";
        }
    }
}
