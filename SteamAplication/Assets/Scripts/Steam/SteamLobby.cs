using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using System;


public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;

    //CallBacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    //Lobbies Callbacks
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdate;
    
    protected Callback<LobbyChatMsg_t> lobbyChatMsg; // added

    public List<CSteamID> lobbyIDs = new List<CSteamID>();

    //Variables
    public ulong CurrentLobbyID;
    private const string HostAddresKey = "HostAddres";
    private CustomNetworkManager manager;


    public int MaxPlayer;

    //GameObject

    private void Start()
    {
        if (!SteamManager.Initialized)
            return;

        if (instance == null) { instance = this; }
        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        
        lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessage);
    }
    public void GetLobbiesList()
    {
        if (lobbyIDs.Count > 0) { lobbyIDs.Clear(); }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }
    private void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.instance.DisplayLobbies(lobbyIDs, result);
    }

    private void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbiesListManager.instance.listOfLobbies.Count > 0) { LobbiesListManager.instance.DestroyLobbies(); }

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);

            SteamMatchmaking.RequestLobbyData(lobbyID);

        }
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        manager.StopHost();
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), HostAddresKey, SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(
             new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s LOBBY");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        int number = SteamMatchmaking.GetNumLobbyMembers(callback.m_steamIDLobby);
        if (number < MaxPlayer)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }
        else
        {
            Debug.Log("Lobby is full");
        }
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        //LobbyNameText.gameObject.SetActive(true);
        //LobbyNameText.text = SteamMatchmaking.GetLobbyData(
        //    new CSteamID(callback.m_ulSteamIDLobby), "name");
        
        if (NetworkServer.active)
            return;

        manager.networkAddress = SteamMatchmaking.GetLobbyData(
             new CSteamID(callback.m_ulSteamIDLobby), HostAddresKey);

        manager.StartClient();
    }
    
    //added
    private void OnLobbyChatMessage(LobbyChatMsg_t callback)
    {
        byte[] data = new byte[4096];

        CSteamID steamIDUser;
        EChatEntryType chatEntryType = EChatEntryType.k_EChatEntryTypeChatMsg;

        SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID,
            out steamIDUser, data, data.Length, out chatEntryType);

        string message = System.Text.Encoding.UTF8.GetString(data);

        FizzyChat.Instance.DisplayChatMessage(SteamFriends.GetFriendPersonaName(steamIDUser), message);
    } 

    public void JoinLobby(CSteamID lobbyID)
    {
        int number = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

        if (number < MaxPlayer)
        {
            SteamMatchmaking.JoinLobby(lobbyID);
        }
        else
        {
            Debug.Log("Lobby is full");
        }
    }

    public void LeaveGame(CSteamID lobbyID)
    {
        SteamMatchmaking.LeaveLobby(lobbyID);
        
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        SteamMatchmaking.DeleteLobbyData(new CSteamID(CurrentLobbyID),"name");
    }
}
