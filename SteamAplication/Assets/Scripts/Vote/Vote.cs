using Mirror;

public class Vote : NetworkBehaviour
{
    [SyncVar]
    public string votePlayer;
    
    [SyncVar]
    public bool vote;

    [SyncVar] public int voteCount;
    public void SetVote(bool _vote, string _votePlayer)
    {
        vote = _vote;
        votePlayer = _votePlayer;
    }
}