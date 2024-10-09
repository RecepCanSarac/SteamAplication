using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Vote : NetworkBehaviour
{
    [SyncVar] public int voteCount;

    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public PlayerObjectController playerGiveVote;

    public List<PlayerObjectController> playerVotes = new List<PlayerObjectController>();

    public bool isAddedList;

    public bool isUseVote;

    public void SetPlayerVoteList(PlayerObjectController playerVoteName, bool isAdded)
    {
        isAddedList = isAdded;
        CmdVoteCount(playerVoteName, isAdded);
    }

    void OnVoteCountUpdated(PlayerObjectController oldValue, PlayerObjectController newValue)
    {
        RpcVoteCount(newValue, isAddedList);
    }

    [Command(requiresAuthority = false)]
    void CmdVoteCount(PlayerObjectController playerVoteName, bool isAdded)
    {
        RpcVoteCount(playerVoteName, isAdded);
    }

    [ClientRpc]
    void RpcVoteCount(PlayerObjectController playerVoteName, bool isAdded)
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