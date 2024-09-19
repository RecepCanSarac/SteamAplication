using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
public class LobbyDataEnttry : MonoBehaviour
{
    public CSteamID LobbyID;
    public string LobbyName;
    public TextMeshProUGUI lobbyNameText;

    public void SetLobbyData()
    {
        if (LobbyName == "")
        {
            lobbyNameText.text = "Empty";
        }
        else
        {
            lobbyNameText.text = LobbyName;
        }
    }

    public void JoinLobby()
    {
        SteamLobby.instance.JoinLobby(LobbyID);
    }


}
