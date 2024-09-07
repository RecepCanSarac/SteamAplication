using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vote : NetworkBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    public bool isActive;
    [SyncVar(hook = nameof(ChangeVoteNumber))] int VoteNumber;

    public Sprite readySprite;
    public Sprite UnreadySprite;

    public void ChangeVoteNumber(int newValue, int oldValue)
    {
        if (isServer)
        {
            VoteUp(newValue);
        }
    }


    [Command]
    public void VoteUp(int newValue)
    {
        VoteUpdate(newValue);
    }

    [Command]
    public void VoteDown(int newValue)
    {
        VoteReduction(newValue);
    }

    [ClientRpc]
    public void VoteUpdate(int newValue)
    {
        VoteNumber += newValue;

        Debug.Log(VoteNumber);
    }
    [ClientRpc]
    public void VoteReduction(int newValue)
    {
        VoteNumber -= newValue;

        Debug.Log(VoteNumber);
    }
}
