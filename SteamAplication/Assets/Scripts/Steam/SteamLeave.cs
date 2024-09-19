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
    public PlayerObjectController currentPlayer;
    
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

        currentPlayer = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>();
    }

    public void LeaveGame()
    {
        /*if(lobbyID != (CSteamID)0)
            SteamLobby.instance.LeaveGame(lobbyID);
        else
            Debug.Log("Lobby ID : " + lobbyID);*/
        
        if (currentPlayer.isLocalPlayer)
        {
            if (isServer)
            {
                Manager.StopHost();
            }
            else
            {
                SteamMatchmaking.LeaveLobby(lobbyID);
                
                Manager.networkAddress = "HostAddress";
                
                Manager.StopClient();
            }
            
            //SceneManager.LoadScene(sceneID);
        }
        
        
    }
}
