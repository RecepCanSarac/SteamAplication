using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;
using TMPro;
public class PlayerObjectController : NetworkBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI ReadText;
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;



    private CustomNetworkManager manager;
    private PlayerCollider PlayerCollider;
    public bool consolActivated = false;




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
    private void Start()
    {
        PlayerCollider = GetComponent<PlayerCollider>();
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        consolActivated = PlayerCollider.isConsolActiveted;
        if (!Ready)
        {
            ReadText.text = "UnReady";
            ReadText.color = Color.red;
        }
        else
        {
            ReadText.text = "Ready";
            ReadText.color = Color.green;
        }
    }
    public void InitializePlayer()
    {
        gameObject.SetActive(true);
    }
    private void PlayerClassUpdate()
    {

    }
    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
        }
        if (isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }
    [Command]
    private void CMdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }
    public void ChangeReady()
    {
        if (isLocalPlayer)
        {
            CMdSetPlayerReady();
        }
    }
    public override void OnStartAuthority()
    {
        CmdSetPlayername(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayername(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);

        NameText.text = this.PlayerName;
    }

    private void PlayerNameUpdate(string OldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }
        if (isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string SceneGame)
    {
        if (isLocalPlayer)
        {
            CmdCanStartGame(SceneGame);
        }
    }
    [Command]
    public void CmdCanStartGame(string SceneGame)
    {
        manager.StartGame(SceneGame);
    }
}

