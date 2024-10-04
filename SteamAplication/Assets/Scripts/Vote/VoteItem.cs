using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class VoteItem : MonoBehaviour
{
    public PlayerObjectController PlayerObjectController;
    public string PlayerName;
    public Text NameText;
    public RawImage PlayerIcon;
    public Text VoteCountText;

    // Bu metod oy sayısını UI'da güncellemek için kullanılır
    public void UpdateCountUI(int newVoteCount)
    {
        VoteCountText.text = newVoteCount.ToString();
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