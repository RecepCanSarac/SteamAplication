using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : MonoBehaviour
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
            VoteCard.gameObject.GetComponent<VoteItem>().playerVoteDC = VoteDC;

            cardItem.PlayerName = card.PlayerName;
            cardItem.NameText.text = cardItem.PlayerName;
            int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)card.PlayerSteamID);
            cardItem.PlayerIcon.texture = cardItem.GetSteamImageAsTexture(ImageID);

            VoteCard.transform.SetParent(VoteCardParend);
            VoteCard.transform.localScale = Vector3.one;

            VoteCard.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                GiveToVote(VoteDC, card);
            });
        }
    }

    public void GiveToVote(Vote cardVote, PlayerObjectController player)
    {
        cardVote.CmdVoteForPlayer(player.PlayerName);
    }

}
