using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
public class PlayerObjectController : NetworkBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI ReadText;
    public TextMeshProUGUI classText;
    public string className;

    public GameObject gUI;

    public bool ısOrderOf = false;
    
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;
    [SyncVar(hook = nameof(ClassNameUpdate))] public string syncedClassName;

    [SyncVar(hook = nameof(OnMovementControll))]
    public bool movementControll = true;
    [SyncVar(hook = nameof(OnCameraControll))]
    public bool cameraControll = false;
    
    private CustomNetworkManager manager;
    private PlayerCollider PlayerCollider;
    
    private PlayerMovmentController _movmentController;

    private CameraController _controller;

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
        _movmentController = GetComponent<PlayerMovmentController>();
        _controller = GetComponent<CameraController>();
        
        PlayerCollider = GetComponent<PlayerCollider>();
        DontDestroyOnLoad(this.gameObject);

        if (isLocalPlayer == false)
        {
            gUI.SetActive(false);
        }
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

        // added
        PlayerNameShow();
    }
    public void InitializePlayer()
    {
        gameObject.SetActive(true);
    }
    public void PlayerNameShow()
    {
        CmdPlayerNameShow();
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
        PlayerNameShow();
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
    
    private void ClassNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.className = newValue;
        }
        if (isClient)
        {
            RpcUpdateUI(PlayerName, newValue);
        }
    }
    
    [Command]
    private void CmdPlayerNameShow()
    {
        RpcUpdateUI(this.PlayerName, className);
    }
    
    [ClientRpc]
    private void RpcUpdateUI(string playerName, string playerClassName)
    {
        NameText.text = playerName;
        classText.text = playerClassName;
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

    //<--------------------------------------------------------------------------------------------------------------------------------------------------------------->
    #region InGameController

    public void SetMovement()
    {
        movementControll = false;
        CmdMovementControll();
    }

    public void SetCamera()
    {
        cameraControll = !cameraControll;
        foreach (PlayerObjectController players in Manager.GamePlayers)
        {
            players.GetComponent<CameraController>().cameraHolder.SetActive(false);
        }
        CmdCameraControll();
    }

    [Command]
    void CmdMovementControll()
    {
        RpcMovementControll(movementControll);
    }

    [Command]
    void CmdCameraControll()
    {
        RpcMovementControll(cameraControll);
    }
    
    void OnMovementControll(bool oldValue, bool newValue)
    {
        RpcMovementControll(newValue);
    }
    
    void OnCameraControll(bool oldValue, bool newValue)
    {
        RpcCameraControll(newValue);
    }

    [ClientRpc]
    void RpcMovementControll(bool newValue)
    {
        _movmentController.enabled = newValue;
    }

    [ClientRpc]
    void RpcCameraControll(bool newValue)
    {
        _controller.enabled = newValue;
        _controller.ChangeCamera();
    }

    #endregion
}
