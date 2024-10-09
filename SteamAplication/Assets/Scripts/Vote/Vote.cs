using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Vote : NetworkBehaviour
{
    [SyncVar]public int voteCount;

    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public string playerGiveVote;
    
    public List<string> playerVotes = new List<string>();
    
    public bool isAddedList;

    public void SetPlayerVoteList(string playerVoteName,bool isAdded)
    {
        isAddedList = isAdded;
        if (isAddedList == false)
        {
            CmdVoteCount(playerVoteName, isAdded);
        }
        else
        {
            Debug.Log("oy kullandın");
        }
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
            isAdded = true;
        }
        else
        {
            playerVotes.Remove(playerVoteName);
            isAdded = false;
        }

        voteCount = playerVotes.Count;
    }
}