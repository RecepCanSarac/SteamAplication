using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : NetworkBehaviour
{
    public static VoteManager Instance;

    public List<PlayerObjectController> votePlayers = new List<PlayerObjectController>();

    public List<VoteItem> currentVoteItems = new List<VoteItem>();

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
        // Her oyuncu için bir oylama kartı oluştur
        foreach (var player in Manager.GamePlayers)
        {
            GameObject VoteCard = Instantiate(VoteCardPrefab);
            VoteItem voteItem = VoteCard.GetComponent<VoteItem>();
            voteItem.PlayerObjectController = player;
            voteItem.PlayerName = player.PlayerName;
            voteItem.NameText.text = player.PlayerName;

            // Oyuncunun Steam Avatarını al
            int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)player.PlayerSteamID);
            voteItem.PlayerIcon.texture = voteItem.GetSteamImageAsTexture(ImageID);

            VoteCard.transform.SetParent(VoteCardParent);
            VoteCard.transform.localScale = Vector3.one;

            currentVoteItems.Add(voteItem);

            if (instancePlayer == player)
            {
                VoteCard.GetComponent<Button>().interactable = false;
                return;
            }
            // Butona tıklama işlevi ekle
            VoteCard.GetComponent<Button>().onClick.AddListener(() => { player.CmdRegisterVote(voteItem.PlayerName); });
        }
    }

    [Server]
    public void ServerHandleVote(string playerNameToVoteFor)
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (player.PlayerName == playerNameToVoteFor)
            {
                var vote = player.GetComponent<Vote>();
                vote.voteCount++;
                RpcUpdateVoteCountUI();
                break;
            }
        }
    }

    [ClientRpc]
    public void RpcUpdateVoteCountUI()
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