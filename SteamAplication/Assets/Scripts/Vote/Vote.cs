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

    public bool isVoteUse = false;

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
        if(isVoteUse == false)
        {
            if (isAdded)
            {
                playerVotes.Add(playerVoteName);
                isAdded = true;
                isVoteUse = true;
            }
        }
        else
        {
            playerVotes.Remove(playerVoteName);
            isAdded = false;
            isVoteUse = false;
        }
        
        voteCount = playerVotes.Count;
    }
}