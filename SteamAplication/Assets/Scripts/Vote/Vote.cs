using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Vote : NetworkBehaviour
{
    CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    public Dictionary<string, bool> VotePairs = new Dictionary<string, bool>();

    public void Update()
    {
        if (isLocalPlayer)
        {
            if (SceneManager.GetActiveScene().name == "Game" && NetworkClient.ready)
            {
                CmdInitializeVotePairs();
            }
        }
    }

    [Command]
    public void CmdInitializeVotePairs()
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (!VotePairs.ContainsKey(player.PlayerName))
            {
                VotePairs.Add(player.PlayerName, false);
            }
        }
    }

    [Command]
    public void CmdVoteForPlayer(string votedPlayerName)
    {
        if (VotePairs.Count == Manager.GamePlayers.Count)
        {
            RpcSetVote(votedPlayerName, this.GetComponent<PlayerObjectController>().PlayerName);
        }
    }

    [ClientRpc]
    void RpcSetVote(string votedPlayerName, string votingPlayerName)
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (player.PlayerName == votedPlayerName)
            {
                player.GetComponent<Vote>().VotePairs[votingPlayerName] = true;
                break;
            }
        }

        if (isLocalPlayer)
        {
            Debug.Log($"{votingPlayerName}, {votedPlayerName} oyuncusuna oy verdi.");
        }
    }
}