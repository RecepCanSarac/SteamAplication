using System.Collections;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLeave : NetworkBehaviour
{
    public int sceneID;

    public CSteamID lobbyID;

    public PlayerObjectController authorityPlayer;
    
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

        authorityPlayer = Manager.GamePlayers[0];
    }

    public void LeaveGame()
    {
        if(lobbyID != (CSteamID)0)
            SteamLobby.instance.LeaveGame(lobbyID);
        else
            Debug.Log("Lobby ID : " + lobbyID);

        StartCoroutine(LeaveGameWithDelay());
    }
    
    IEnumerator LeaveGameWithDelay()
    {
        Manager.offlineScene = "";

        Debug.Log(authorityPlayer.PlayerName);
        
        if (authorityPlayer)
        {
            if (isServer)
            {
                Manager.StopHost();
            }
            else
            {
                Manager.StopClient();
            }
        }
        
        Manager.networkAddress = "HostAddress";

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneID);
    }
}
