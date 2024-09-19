using Mirror;
using UnityEngine;

public class SteamHostMigrationManager : NetworkBehaviour
{
    private CustomNetworkManager manager;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>() as CustomNetworkManager;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        AssignNewHost();
    }

    private void AssignNewHost()
    {
        if (NetworkServer.connections.Count > 1)
        {
            NetworkConnection newHostConnection = null;
            
            foreach (var conn in NetworkServer.connections)
            {
                if (conn.Value != connectionToClient)
                {
                    newHostConnection = conn.Value;
                    break;
                }
            }
            if (newHostConnection != null)
            {
                NotifyPlayersOfNewHost(newHostConnection);
                TransferStateToNewHost(newHostConnection);
            }
        }
        else
        {
            Debug.Log("Yeterli oyuncu yok, oyun sonlandırılıyor.");
            manager.StopHost();
        }
    }
    
    private void NotifyPlayersOfNewHost(NetworkConnection newHostConnection)
    {
        foreach (var conn in NetworkServer.connections)
        {
            Debug.Log("hey notifiy");
        }
    }

    private void TransferStateToNewHost(NetworkConnection newHostConnection)
    {
        Debug.Log("Changed state trasnfer");
    }
}
