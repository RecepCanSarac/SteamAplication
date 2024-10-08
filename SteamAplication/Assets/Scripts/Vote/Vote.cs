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

    [Command(requiresAuthority = false)]
    public void PlayerVotesUpdated(Vote vote)
    {
        string playerName = vote.GetComponent<PlayerObjectController>().PlayerName;
        
        if (!vote.playerVotes.Contains(playerName))
        {
            vote.playerVotes.Add(playerName);
            vote.votesReceived++;
        }
        else
        {
            vote.playerVotes.Remove(playerName);
            vote.votesReceived--;
        }
    }
}