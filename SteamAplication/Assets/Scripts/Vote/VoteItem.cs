using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoteItem : NetworkBehaviour
{
    public PlayerObjectController PlayerObjectController;
    public string PlayerName;
    public TextMeshProUGUI NameText;
    public RawImage PlayerIcon;
    public TextMeshProUGUI VoteCountText;
    public Vote playerVote;

    public void UpdateCountUI(int newVoteCount)
    {
        VoteCountText.text = newVoteCount.ToString();
    }

    public void GetPlayerName()
    {
        Debug.Log(PlayerName);
        PlayerObjectController player = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>();

        playerVote = PlayerObjectController.GetComponent<Vote>();

        if (!playerVote.playerVotes.Contains(player) && player.GetComponent<Vote>().isUseVote == false)
        {
            playerVote.SetPlayerVoteList(player, true);
            player.GetComponent<Vote>().isUseVote = true;
        }

        else
        {
            playerVote.SetPlayerVoteList(player, false);
            player.GetComponent<Vote>().isUseVote = false;
        }
    }

    public Texture2D GetSteamImageAsTexture(int ImageID)
    {
        Texture2D texture = null;
        uint ImageWidth;
        uint ImageHeight;
        bool success = SteamUtils.GetImageSize(ImageID, out ImageWidth, out ImageHeight);

        if (success)
        {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];
            success = SteamUtils.GetImageRGBA(ImageID, Image, (int)(ImageWidth * ImageHeight * 4));

            if (success)
            {
                texture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(Image);
                texture.Apply();
            }
        }

        return texture;
    }
}