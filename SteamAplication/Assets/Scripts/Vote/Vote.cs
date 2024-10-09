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
    public bool isAddedList;

    public void SetPlayerVoteList(string playerVoteName,bool isAdded)
    {
        isAddedList = isAdded;
        CmdVoteCount(playerVoteName, isAdded);
    }
    void OnVoteCountUpdated(string oldValue, string newValue)
    {
        RpcVoteCount(newValue,isAddedList);
    }

    [Command(requiresAuthority = false)]
    void CmdVoteCount(string playerVoteName, bool isAdded)
    {
        RpcVoteCount(playerVoteName,isAdded);
    }
    
    [ClientRpc]
    void RpcVoteCount(string playerVoteName, bool isAdded)
    {
        if (isAdded)
        {
            playerVotes.Add(playerVoteName);
            voteCount++;
            isVote = true;
            isAdded = true;
        }
        else
        {
            playerVotes.Remove(playerVoteName);
            voteCount--;
            isVote = false;
            isAdded = false;
        }
    }
}