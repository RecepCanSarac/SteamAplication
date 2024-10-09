using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Vote : NetworkBehaviour
{
    [SyncVar]public int voteCount;

    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public string playerGiveVote;
    
    public List<string> playerVotes = new List<string>();

    public void SetPlayerVoteList(string playerVoteName)
    {
        CmdVoteCount(playerVoteName);
    }
    void OnVoteCountUpdated(string oldValue, string newValue)
    {
        RpcVoteCount(newValue);
    }

    [Command(requiresAuthority = false)]
    void CmdVoteCount(string playerVoteName)
    {
        RpcVoteCount(playerVoteName);
    }
    
    [ClientRpc]
    void RpcVoteCount(string playerVoteName)
    {
        playerVotes.Add(playerVoteName);
        voteCount++;
    }

    public void SetList(PlayerObjectController playerObjectController)
    {
        playerVotes.Add(playerObjectController.PlayerName);
    }
}