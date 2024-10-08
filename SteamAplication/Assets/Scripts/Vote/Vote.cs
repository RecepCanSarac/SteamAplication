using System.Collections.Generic;
using Mirror;

public class Vote : NetworkBehaviour
{
    [SyncVar] public string votePlayer;
    
    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    
    public int voteCount;
    public void SetVote(bool _vote, string _votePlayer, int _votecount)
    {
        if (isServer)
        {
            votePlayer = _votePlayer;
            voteCount = _votecount;
        }
    }
    private void OnVoteCountUpdated(int oldValue, int newValue)
    {
        VoteManager.Instance.UpdateVoteCountUI();
    }
    [ClientRpc]
    public void RpcUpdateVoteUI(int newVoteCount)
    {
        VoteManager.Instance.UpdateVoteCountUI();
    }
}