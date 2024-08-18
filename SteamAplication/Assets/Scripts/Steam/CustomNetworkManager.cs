using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using Unity.VisualScripting;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } =
        new List<PlayerObjectController>();
    
    public List<SOClass> MangmentClass = new List<SOClass>();
    
    public List<ClassItem> classItems = new List<ClassItem>();

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
        
            FizzyChat.Instance.Joined();

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }
    
    // added
    public override void ServerChangeScene(string newSceneName)
    {
        if (newSceneName.StartsWith("Game"))
        {
            AssignClassesToPlayers();
        }
            
        base.ServerChangeScene(newSceneName);
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
    
    private void AssignClassesToPlayers()
    {
        List<int> assignedIndexes = new List<int>();
        foreach (var player in GamePlayers)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, MangmentClass.Count);
            }
            while (assignedIndexes.Contains(randomIndex));

            assignedIndexes.Add(randomIndex);

            PlayerClass playerClass = player.GetComponent<PlayerClass>();

            playerClass.Type = MangmentClass[randomIndex].ClassType;

            playerClass.Class = MangmentClass[randomIndex];

            playerClass.ClassTextMethod(MangmentClass[randomIndex].ClassType);

            player.className = MangmentClass[randomIndex].ClassType.ToString();

            player.PlayerNameShow();
        }
    }

    public void AddListClass(SOClass userClass)
    {
        MangmentClass.Add(userClass);
    }
}
