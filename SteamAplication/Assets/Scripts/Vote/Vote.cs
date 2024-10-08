using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Vote : NetworkBehaviour
{
    [SyncVar]public int voteCount;

    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public List<string> playerVotes = new List<string>();


    public void SetPlayerVoteList(List<string> playerVoteName)
    {
        CmdVoteCount(playerVoteName);
    }
    void OnVoteCountUpdated(List<string> oldValue, List<string> newValue)
    {
        RpcVoteCount(newValue);
    }

    [Command]
    void CmdVoteCount(List<string> playerVoteName)
    {
        RpcVoteCount(playerVoteName);
    }
    
    [ClientRpc]
    void RpcVoteCount(List<string> playerVoteName)
    {
        playerVotes = playerVoteName;
        voteCount++;
    }
}