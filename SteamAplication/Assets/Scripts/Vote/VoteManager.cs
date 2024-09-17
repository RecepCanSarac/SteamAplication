using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : NetworkBehaviour
{
    public static VoteManager Instance;

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

    CustomNetworkManager manager;

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

            // Adding the voting functionality with proper authority handling
            VoteCard.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                GiveToVote(VoteDC, card, cardItem);
            });
        }
    }

    public void GiveToVote(Vote cardVote, PlayerObjectController player, VoteItem ıtem)
    {
        string oldvalue = player.PlayerName;
        if (cardVote.votePlayer != oldvalue)
        {
            cardVote.vote = !cardVote.vote;
            cardVote.SetVote(cardVote.vote, player.PlayerName);
            CheckPlayerVote();
        }
        else
        {
            Debug.Log("Bu oyuncuya verdin zaten");
        }
        ıtem.voteCount = cardVote.voteCount;
        ıtem.SetPlayerValues();
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
    }
}