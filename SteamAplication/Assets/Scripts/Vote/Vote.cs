using System.Collections.Generic;
using Mirror;

public class Vote : NetworkBehaviour
{
    [SyncVar] public int votesReceived;
    
    [SyncVar]
    private PlayerObjectController votedFor;

    public List<string> playerVotes;
    
    [Command(requiresAuthority = false)]
    public void CmdRegisterVote(Vote vote)
    {
        VoteManager.Instance.ServerHandleVote(vote);
    }

    public void PlayerVotesUpdated(Vote vote)
    {
        string playerName = vote.GetComponent<PlayerObjectController>().PlayerName;
        
        if (!playerVotes.Contains(playerName))
        {
            playerVotes.Add(playerName);
            vote.votesReceived++;
        }
        else
        {
            playerVotes.Remove(playerName);
            vote.votesReceived--;
        }
    }
}