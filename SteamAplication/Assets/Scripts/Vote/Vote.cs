using Mirror;

public class Vote : NetworkBehaviour
{
    [SyncVar]
    public string votePlayer;
    
    [SyncVar]
    public bool vote;

    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public int voteCount;
    public void SetVote(bool _vote, string _votePlayer, int _votecount)
    {
        if (isServer)
        {
            vote = _vote;
            votePlayer = _votePlayer;
            voteCount = _votecount;

            RpcUpdateVoteUI(voteCount);
        }
    }
    
    private void OnVoteCountUpdated(int oldValue, int newValue)
    {
        VoteManager.Instance.UpdateVoteCountUI();
    }

    [ClientRpc]
    private void RpcUpdateVoteUI(int newVoteCount)
    {
        VoteManager.Instance.UpdateVoteCountUI();
    }
}