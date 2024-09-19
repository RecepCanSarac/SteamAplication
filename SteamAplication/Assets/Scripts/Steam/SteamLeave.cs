using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLeave : NetworkBehaviour
{
    public int sceneID;

    public CSteamID lobbyID;
    
    #region Singleton

    private CustomNetworkManager _manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (_manager != null) return _manager;
            return _manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    #endregion

    void Start()
    {
        lobbyID = (CSteamID)SteamLobby.instance.CurrentLobbyID;
    }

    public void LeaveGame()
    {
        if(lobbyID != (CSteamID)0)
            SteamLobby.instance.LeaveGame(lobbyID);
        else
            Debug.Log("Lobby ID : " + lobbyID);

        if (isServer)
        {
            Manager.StopClient();
        }

        if (isLocalPlayer)
        {
            Manager.StopHost();
        }

        Manager.networkAddress = "HostAddress";

        SceneManager.LoadScene(sceneID);
    }
}
