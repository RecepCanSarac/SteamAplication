using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FizzyChat : MonoBehaviour
{
    public static FizzyChat Instance;

    void Awake() => Instance = this;

    public TMP_InputField chatMessage;

    public GameObject text;

    public Transform content;

    public Scrollbar verticalBar;

    private List<GameObject> textList = new List<GameObject>();

    public void EnterText()
    {
        if (!string.IsNullOrEmpty(chatMessage.text))
        {
            byte[] chatMessageBytes = System.Text.Encoding.UTF8.GetBytes(chatMessage.text);

            CSteamID ıd = new CSteamID(SteamLobby.instance.CurrentLobbyID);

            SteamMatchmaking.SendLobbyChatMsg(ıd, chatMessageBytes, chatMessageBytes.Length);

            chatMessage.text = "";
        }
        else
            Debug.Log("Text is empty or null");
    }

    public void Joined(string playerName)
    {
        chatMessage.text = " " + playerName + " joined";
        
        byte[] chatMessageBytes = System.Text.Encoding.UTF8.GetBytes(chatMessage.text);

        CSteamID ıd = new CSteamID(SteamLobby.instance.CurrentLobbyID);

        SteamMatchmaking.SendLobbyChatMsg(ıd, chatMessageBytes, chatMessageBytes.Length);

        chatMessage.text = "";
    }

    public void DisplayChatMessage(string userName, string message)
    {
        GameObject _text = Instantiate(text, content);

        _text.GetComponent<TMP_Text>().text = $"{userName} : {message}";
        
        textList.Add(_text);

        if (verticalBar.IsActive())
            verticalBar.value = 0;

        if (textList.Count > 6)
        {
            Destroy(textList[0]);
            textList.RemoveAt(0);
        }
    }
}