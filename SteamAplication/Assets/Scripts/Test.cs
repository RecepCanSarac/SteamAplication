using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    
    public List<PlayerObjectController> GamePlayers =
        new List<PlayerObjectController>();
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID =
                (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                    (CSteamID)SteamLobby.instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }
    
    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
    
    public void ReturnToMainMenu(NetworkConnection conn)
    {
        if (conn != null && conn.identity != null)
        {
            if (conn.identity.isServer)
            {
                StopHost();  // Sunucuyu kapat
                Debug.Log("Host stopped.");
            }
            else
            {
                StopClient();
                Debug.Log("Client disconnected.");
            }
        }

        SteamMatchmaking.LeaveLobby((CSteamID)SteamLobby.instance.CurrentLobbyID);

        SceneManager.LoadScene("MainMenu");
    }
}
