using System;
using Steamworks;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoteItem : NetworkBehaviour
{
    public Vote playerVoteDC;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI voteCountText;
    public RawImage PlayerIcon;

    public int voteCount;
    
    public bool received;
    public string PlayerName;
    
    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    public PlayerObjectController PlayerObjectController;

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void SetPlayerValues()
    {
        //NameText.text = PlayerName;
        //if (!AvatarReceived) { GetPlayerIcon(); }
    }

    public void UpdateCountUI(int newVoteCount)
    {
        TargetVoteCountUI(newVoteCount);
    }

    [TargetRpc]
    public void TargetVoteCountUI(int newVoteCount)
    {
        voteCount = newVoteCount + 1;
        Debug.Log(voteCount);
        voteCountText.text = "Count: " + voteCount;
    }
    
    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerObjectController.PlayerSteamID);
        if (ImageID == -1) { return; }
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }
    public Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();

                Color32[] pixels = texture.GetPixels32();
                for (int y = 0; y < height / 2; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int topIndex = y * (int)width + x;
                        int bottomIndex = ((int)height - 1 - y) * (int)width + x;
                        Color32 temp = pixels[topIndex];
                        pixels[topIndex] = pixels[bottomIndex];
                        pixels[bottomIndex] = temp;
                    }
                }
                texture.SetPixels32(pixels);
                texture.Apply();
            }
        }
        received = true;
        return texture;
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerObjectController.PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }
}
