using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : NetworkBehaviour
{
    public static VoteManager Instance;

    public List<VoteItem> currentVoteItems = new List<VoteItem>();

    public List<string> playerVotesNames = new List<string>();

    private string currentplayerName;

    public GameObject VoteCardPrefab;
    public Transform VoteCardParent;

    public PlayerObjectController instancePlayer;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CreateVoteCardItem();
    }

    private void CreateVoteCardItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            GameObject VoteCard = Instantiate(VoteCardPrefab);
            VoteItem voteItem = VoteCard.GetComponent<VoteItem>();
            voteItem.PlayerObjectController = player;
            voteItem.PlayerName = player.PlayerName;
            voteItem.NameText.text = player.PlayerName;

            int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)player.PlayerSteamID);
            voteItem.PlayerIcon.texture = voteItem.GetSteamImageAsTexture(ImageID);

            VoteCard.transform.SetParent(VoteCardParent);
            VoteCard.transform.localScale = Vector3.one;

            currentVoteItems.Add(voteItem);

            if (instancePlayer == player)
            {
                VoteCard.GetComponent<Button>().interactable = false;
            }
            else
            {
                VoteCard.GetComponent<Button>().onClick.AddListener(() => { 
                    ServerHandleVote(voteItem.PlayerObjectController.PlayerName,voteItem.PlayerObjectController.GetComponent<Vote>()); });
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void ServerHandleVote(string name, Vote vote)
    {
        Debug.Log(name);
        vote.SetPlayerVoteList(name);
        RpcUpdateVoteCountUI();
    }

    [ClientRpc]
    void RpcUpdateVoteCountUI()
    {
        UpdateVoteCountUI();
    }

    public void UpdateVoteCountUI()
    {
        foreach (var player in Manager.GamePlayers)
        {
            foreach (var voteItem in currentVoteItems)
            {
                if (voteItem.PlayerName == player.PlayerName)
                {
                    voteItem.UpdateCountUI(player.GetComponent<Vote>().voteCount);
                }
            }
        }
    }
}