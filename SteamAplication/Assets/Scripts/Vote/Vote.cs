using Mirror;

public class Vote : NetworkBehaviour
{
    [SyncVar]
    public string votePlayer;
    
    [SyncVar]
    public bool vote;

    [SyncVar(hook = nameof(OnVoteCountChanged))]
    public int voteCount;
    public void SetVote(bool _vote, string _votePlayer)
    {
        if (isServer)
        {
            voteCount += _vote ? 1 : 0;
            votePlayer = _votePlayer;
        }
    }
    
    void OnVoteCountChanged(int oldValue, int newValue)
    {
        RpcUpdateVoteCount(newValue);
    }
    
    [ClientRpc]
    private void RpcUpdateVoteCount(int newValue)
    {
        VoteItem voteItem = VoteManager.Instance.currentItem;
        voteItem.UpdateVoteCountUI(newValue);
    }
}