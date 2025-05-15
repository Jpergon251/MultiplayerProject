using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace NetworkScripts
{
    public class NetworkRelayManager : MonoBehaviour
    {
        public static NetworkRelayManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async void StartHostWithRelay(Allocation allocation)
        {
            try
            {

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                RelayServerData relayServerData = new RelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.ConnectionData,  // Cliente
                    allocation.ConnectionData,  // Host (a falta de HostConnectionData)
                    allocation.Key,
                    true
                );

                transport.SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                // Aquí deberías guardar el joinCode en el Lobby (como se explicó antes)
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }
        }

        public async void JoinHostWithRelay(string joinCode)
        {
            await Task.Delay(2000);

            try
            {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                RelayServerData relayServerData = new RelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.ConnectionData,       // El connectionData del cliente
                    allocation.HostConnectionData,   // El connectionData del host
                    allocation.Key,                  // La clave HMAC
                    true                           // Usa DTLS
                );

                transport.SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                
                
                
            }
        }
    }
}
