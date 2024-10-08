using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : NetworkBehaviour
{
    public static VoteManager Instance;

    public List<VoteItem> currentVoteItems = new List<VoteItem>();

    public Dictionary<string, bool> playerVotes = new Dictionary<string, bool>();


    public List<string> playerVotesNames = new List<string>();

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
                VoteCard.GetComponent<Button>().onClick.AddListener(() => { player.CmdRegisterVote(player.PlayerName); });
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void ServerHandleVote(string playerNameToVoteFor, string voterName)
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (player.PlayerName == playerNameToVoteFor)
            {
                if (!playerVotes.ContainsKey(player.PlayerName) && player.voting == false)
                {
                    playerVotes.Add(player.PlayerName, true);
                    playerVotesNames.Add(player.PlayerName);

                    var vote = player.GetComponent<Vote>();
                    vote.votesReceived++;
                    player.voting = true;
                    RpcUpdateVoteCountUI();
                    return;
                }

                if (playerVotes.ContainsKey(player.PlayerName) && player.voting == true)
                {
                    var vote = player.GetComponent<Vote>();
                    vote.votesReceived--;
                    player.voting = false;
                    playerVotes.Remove(player.PlayerName);
                    playerVotesNames.Remove(player.PlayerName);
                    RpcUpdateVoteCountUI();
                    return;
                }
            }
        }
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
                    voteItem.UpdateCountUI(player.GetComponent<Vote>().votesReceived);
                }
            }
        }
    }
}