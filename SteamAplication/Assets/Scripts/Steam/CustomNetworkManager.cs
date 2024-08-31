using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using Random = UnityEngine.Random;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;

    public List<PlayerObjectController> GamePlayers =
        new List<PlayerObjectController>();

    public List<SOClass> MangmentClass = new List<SOClass>();

    public List<string> className = new List<string>();

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

            StartCoroutine(JoinMessage(GamePlayerInstance));
        }
    }

    public void SetList()
    {
        foreach (PlayerObjectController players in GamePlayers)
        {
            players.GetComponent<PlayerClass>().className = className;
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
        if (MangmentClass == null || MangmentClass.Count == 0)
        {
            Debug.Log("MangmentClass list is empty. Cannot assign classes to players.");
            return;
        }

        List<int> assignedIndexes = new List<int>();
        foreach (var player in GamePlayers)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, MangmentClass.Count);
            } while (assignedIndexes.Contains(randomIndex));

            assignedIndexes.Add(randomIndex);

            PlayerClass playerClass = player.GetComponent<PlayerClass>();

            /*playerClass.Type = MangmentClass[randomIndex].ClassType;

            playerClass.Class = MangmentClass[randomIndex];

            playerClass.ClassTextMethod(MangmentClass[randomIndex].ClassType);*/

            player.className = MangmentClass[randomIndex].ClassType.ToString();

            player.PlayerNameShow();
        }
    }

    public void AddListClass(SOClass userClass)
    {
        MangmentClass.Add(userClass);
    }

    public void UpdatedClassPlayer(string _className)
    {
        className.Add(_className);
    }

    IEnumerator JoinMessage(PlayerObjectController GamePlayerInstance)
    {
        yield return new WaitForSeconds(1);

        FizzyChat.Instance.Joined(GamePlayerInstance.PlayerName);
    }
}