using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : NetworkBehaviour
{
    public static VoteManager instance;
    public GameObject VotePrefab;
    public GameObject VoteGroup;
    public GameObject LocalPlayerObject;
    private List<Vote> items = new List<Vote>();
    private PlayerObjectController LocalPlayerController;
    CustomNetworkManager manager;

    public bool isVote = true;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    public void FindPlayerObject()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");

        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void Start()
    {
        CreatedVoitItems();
    }
    public void CreatedVoitItems()
    {
        FindPlayerObject();
        foreach (PlayerObjectController item in Manager.GamePlayers)
        {
            GameObject newVotePrefab = Instantiate(VotePrefab);
            VoidItem newVote = newVotePrefab.GetComponent<VoidItem>();
            newVote.NameText.text = item.PlayerName;
            newVote.Vote.PlayerName = item.PlayerName;
            newVote.Vote.ConnectionID = item.ConnectionID;
            newVote.Vote.PlayerSteamID = item.PlayerSteamID;
            newVote.SetPlayerValues();

            newVotePrefab.transform.SetParent(VoteGroup.transform);
            newVotePrefab.transform.localScale = Vector3.one;
            newVotePrefab.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                UseToVote(newVote.Vote, newVote);
            });
            items.Add(newVote.Vote);
        }
    }
    public void UseToVote(Vote vote, VoidItem item)
    {
        if (vote == null) return;
        isVote = !isVote;
        if (isVote == true)
        {
            vote.isActive = true;
            vote.VoteUp(1);
        }
        else
        {
            vote.isActive = false;
            vote.VoteDown(1);
        }

        item.ChangeReadyStatus(isVote);
    }
}
