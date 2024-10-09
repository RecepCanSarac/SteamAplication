using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Vote : NetworkBehaviour
{
    [SyncVar] public int voteCount;

    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public string playerGiveVote;

    public List<string> playerVotes = new List<string>();
    
    public string currentVote;

    public bool isAddedList;

    public void SetPlayerVoteList(string playerVoteName)
    {
        if (currentVote == playerVoteName)
        {
            CmdVoteCount(playerVoteName, false);
            currentVote = null;
        }
        else
        {
            if (currentVote != null)
            {
                CmdVoteCount(currentVote, false);
            }
            
            CmdVoteCount(playerVoteName, true);
            currentVote = playerVoteName;
        }
    }

    void OnVoteCountUpdated(string oldValue, string newValue)
    {
        RpcVoteCount(newValue, isAddedList);
    }

    [Command(requiresAuthority = false)]
    void CmdVoteCount(string playerVoteName, bool isAdded)
    {
        RpcVoteCount(playerVoteName, isAdded);
    }

    [ClientRpc]
    void RpcVoteCount(string playerVoteName, bool isAdded)
    {
        if (isAdded)
        {
            if (!playerVotes.Contains(playerVoteName))
            {
                playerVotes.Add(playerVoteName);
            }
        }
        else
        {
            if (playerVotes.Contains(playerVoteName))
            {
                playerVotes.Remove(playerVoteName);
            }
        }

        voteCount = playerVotes.Count;
    }
}