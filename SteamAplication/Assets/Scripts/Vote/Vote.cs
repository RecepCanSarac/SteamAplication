using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Vote : NetworkBehaviour
{
    [SyncVar]public int voteCount;

    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public string playerGiveVote;
    
    public List<string> playerVotes = new List<string>();

    public bool isVote = false;

    public void SetPlayerVoteList(string playerVoteName)
    {
        CmdVoteCount(playerVoteName,isVote);
    }
    void OnVoteCountUpdated(string oldValue, string newValue)
    {
        RpcVoteCount(newValue,isVote);
    }

    [Command(requiresAuthority = false)]
    void CmdVoteCount(string playerVoteName, bool isHave)
    {
        RpcVoteCount(playerVoteName,isHave);
    }
    
    [ClientRpc]
    void RpcVoteCount(string playerVoteName, bool isHave)
    {
        playerVotes.Add(playerVoteName);
        if (isHave == false)
        {
            voteCount++;
            
            isVote = true;
        }
        else
        {
            voteCount--;
        }
    }
}