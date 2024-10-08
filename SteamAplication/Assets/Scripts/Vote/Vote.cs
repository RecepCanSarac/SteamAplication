using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Vote : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnVoteCountUpdated))]
    public int voteCount;

    private HashSet<string> voters = new HashSet<string>();

    public void SetVote(bool _vote, string _votePlayer, int _votecount)
    {
        if (isServer)
        {
            //votePlayer = _votePlayer;
            voteCount = _votecount;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdRegisterVote(string voterName)
    {
        if (!voters.Contains(voterName))
        {
            ServerHandleVote(voterName);
        }
        else
        {
            Debug.Log("Bu oyuncu zaten oy verdi.");
        }
    }

    [Server]
    public void ServerHandleVote(string voterName)
    {
        voters.Add(voterName);
        voteCount++;
        RpcUpdateVoteCountUI();
    }

    private void OnVoteCountUpdated(int oldValue, int newValue)
    {
        VoteManager.Instance.UpdateVoteCountUI();
    }

    [ClientRpc]
    public void RpcUpdateVoteCountUI()
    {
        VoteManager.Instance.UpdateVoteCountUI();
    }
}