using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : NetworkBehaviour
{
    public static VoteManager Instance;

    List<PlayerObjectController> votePlayers = new List<PlayerObjectController>();

    public VoteItem currentItem;

    public List<VoteItem> currentVoteItem = new List<VoteItem>();

    #region Singleton

    CustomNetworkManager manager;

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

    #endregion

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

    public GameObject VoteCardPrefab;
    public Transform VoteCardParend;

    private void Start()
    {
        CreateVoteCardItem();
    }

    private void CreateVoteCardItem()
    {
        foreach (Transform child in VoteCardParend)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in Manager.GamePlayers)
        {
            GameObject VoteCard = Instantiate(VoteCardPrefab);
            VoteItem cardItem = VoteCard.gameObject.GetComponent<VoteItem>();
            cardItem.PlayerObjectController = card;
            Vote VoteDC = card.GetComponent<Vote>();
            cardItem.playerVoteDC = VoteDC;

            cardItem.PlayerName = card.PlayerName;
            cardItem.NameText.text = cardItem.PlayerName;
            int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)card.PlayerSteamID);
            cardItem.PlayerIcon.texture = cardItem.GetSteamImageAsTexture(ImageID);

            VoteCard.transform.SetParent(VoteCardParend);
            VoteCard.transform.localScale = Vector3.one;

            currentVoteItem.Add(cardItem);

            VoteCard.gameObject.GetComponent<Button>().onClick
                .AddListener(() =>
                {

                    if (card.isLocalPlayer == true)
                    {
                        GiveToVote(VoteDC, card, cardItem);
                    }
                });
        }
    }

    public void GiveToVote(Vote cardVote, PlayerObjectController player, VoteItem item)
    {
        if(!isClient)
            CmdGiveToVote(player.netIdentity);
    }

    [Command]
    public void CmdGiveToVote(NetworkIdentity playerIdentity)
    {
        var player = playerIdentity.GetComponent<PlayerObjectController>();
        var cardVote = player.GetComponent<Vote>();

        if (!votePlayers.Contains(player))
        {
            cardVote.vote = !cardVote.vote;
            cardVote.SetVote(cardVote.vote, player.PlayerName, cardVote.voteCount);
            CheckPlayerVote();
            votePlayers.Add(player);
        }
    }

    public void CheckPlayerVote()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            var voteInstance = player.GetComponent<Vote>();

            if (player.PlayerName == voteInstance.votePlayer)
            {
                voteInstance.voteCount++;
            }
        }

        UpdateVoteCountUI();
    }


    public void UpdateVoteCountUI()
    {
        for (int i = 0; i < Manager.GamePlayers.Count; i++)
        {
            currentVoteItem[i].UpdateVoteCountUI(currentVoteItem[i].voteCount);
        }
    }
}