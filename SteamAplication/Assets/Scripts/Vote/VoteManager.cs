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
                if (card.isLocalPlayer)
                {
                    GiveToVote(VoteDC, card);
                }
                else
                {
                    // Inform the server to handle voting for this player
                    CmdRequestVote(card.PlayerName);
                }
            });
        }
    }

    public void GiveToVote(Vote cardVote, PlayerObjectController player)
    {
        if (isLocalPlayer)
        {
            cardVote.CmdVoteForPlayer(player.PlayerName);
        }
    }
    
    public void CmdRequestVote(string playerName)
    {
        // Perform the voting on the server-side, ensuring that authority is respected
        PlayerObjectController votingPlayer = FindPlayerByName(playerName); // Assuming a method that finds the player
        if (votingPlayer != null)
        {
            votingPlayer.GetComponent<Vote>().CmdVoteForPlayer(playerName);
        }
    }

    private PlayerObjectController FindPlayerByName(string playerName)
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (player.PlayerName == playerName)
            {
                return player;
            }
        }
        return null;
    }
}
