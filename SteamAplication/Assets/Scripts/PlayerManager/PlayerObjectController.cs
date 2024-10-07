using System;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerObjectController : NetworkBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI ReadText;
    public TextMeshProUGUI classText;

    public GameObject house;

    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;

    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string PlayerName;

    [SyncVar(hook = nameof(PlayerReadyUpdate))]
    public bool Ready;

    [SyncVar(hook = nameof(ClassNameUpdate))]
    public string syncedClassName;

    private CustomNetworkManager manager;
    private PlayerCollider PlayerCollider;

    public bool consolActivated = false;

    public bool voting = false;

    private bool isSetPlayer = false;

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

    [SyncVar] public bool hasVoted = false;

    [Command(requiresAuthority = false)]
    public void CmdRegisterVote(string playerNameToVoteFor)
    {
        VoteManager.Instance.ServerHandleVote(playerNameToVoteFor, this.PlayerName);
    }

    [Command]
    public void CmdGiveToVote(string playerNameToVoteFor)
    {
        if (isLocalPlayer)
        {
            //VoteManager.Instance.ServerHandleVote(playerNameToVoteFor);
            hasVoted = true;
        }
        else
        {
            Debug.LogWarning("Oy verme işlemi yetkisiz oyuncu tarafından yapıldı!");
        }
    }

    private void Start()
    {
        ClassGenerator.Instance.SetList();
        foreach (var data in ClassGenerator.Instance.DataList)
        {
            Debug.Log(data.ClassName);
        }

        PlayerCollider = GetComponent<PlayerCollider>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void Leave()
    {
        if (isServer)
        {
            CustomNetworkManager.singleton.StopHost();
            return;
        }

        CustomNetworkManager.singleton.StopClient();
    }

    private void OnDestroy()
    {
        // Oyuncu nesnesi yok edilirken yapılacak işlemler
        Debug.Log("Player object is being destroyed.");
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

        if (isLocalPlayer)
        {
            if (SceneManager.GetActiveScene().name == "Game" && isSetPlayer == false)
            {
                VoteManager.Instance.instancePlayer = this;
                isSetPlayer = true;
            }
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
        if (isLocalPlayer && isClient && NetworkClient.ready)
        {
            CmdPlayerNameShow();
            classText.text = syncedClassName;
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

    public void CanStartGame(string SceneGame)
    {
        if (isLocalPlayer)
        {
            CmdCanStartGame(SceneGame);
        }
    }

    public void ChangeReady()
    {
        if (isLocalPlayer)
        {
            CMdSetPlayerReady();
        }
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

    private void ClassNameUpdate(string oldValue, string newValue)
    {
        CmdUpdateClass(newValue);

        if (isServer)
        {
            RpcUpdateUI(PlayerName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneGame)
    {
        manager.StartGame(SceneGame);
    }

    [Command]
    private void CMdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    [Command]
    private void CmdPlayerNameShow()
    {
        RpcUpdateUI(this.PlayerName);
    }

    [Command]
    private void CmdSetPlayername(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    [Command]
    void CmdUpdateClass(string value)
    {
        RpcUpdateClass(value);
        classText.text = syncedClassName;
    }

    [ClientRpc]
    private void RpcUpdateUI(string playerName)
    {
        NameText.text = playerName;
    }

    [ClientRpc]
    void RpcUpdateClass(string classType)
    {
        syncedClassName = classType;
        classText.text = syncedClassName;
    }

    public void SetHouse(Vector3 pos)
    {
        Vector3 offsetRotate = new Vector3(-8.95f, 0f, 0f);
        GameObject houseInstance = Instantiate(house, pos, Quaternion.Euler(offsetRotate));
        houseInstance.GetComponent<House>().PlayerObjectController = this;
        houseInstance.GetComponent<House>().type = syncedClassName;
        Vector3 dir = houseInstance.transform.position - transform.position;

        dir.y = 0f;
        houseInstance.transform.rotation = Quaternion.LookRotation(new Vector3(-8.95f, dir.y, dir.z));
    }

    public void ActivetedHouse(string type)
    {
        CMDActivetedHouse(type);
    }

    [Command]
    public void CMDActivetedHouse(string type)
    {
        RpcActivetedHouse(type);
    }

    [ClientRpc]
    void RpcActivetedHouse(string type)
    {
        if (Enum.TryParse(type, out ClassType classType))
        {
            switch (classType)
            {
                case ClassType.Doctor:
                    Debug.Log("Doctor IsActive (Client)");
                    break;
                case ClassType.Detective:
                    Debug.Log("Detective IsActive (Client)");
                    break;
                case ClassType.Seer:
                    Debug.Log("Seer IsActive (Client)");
                    break;
                case ClassType.Armored:
                    Debug.Log("Armored IsActive (Client)");
                    break;
                case ClassType.Confessor:
                    Debug.Log("Confessor IsActive (Client)");
                    break;
                case ClassType.Thief:
                    Debug.Log("Thief IsActive (Client)");
                    break;
                case ClassType.Trapper:
                    Debug.Log("Trapper IsActive (Client)");
                    break;
                case ClassType.Buffoon:
                    Debug.Log("Buffoon IsActive (Client)");
                    break;
                case ClassType.Lookout:
                    Debug.Log("Lookout IsActive (Client)");
                    break;
                case ClassType.Killer:
                    Debug.Log("Killer IsActive (Client)");
                    break;
                default:
                    Debug.Log("Unknown class type");
                    break;
            }
        }
        else
        {
            Debug.Log("Invalid class type string: " + type);
        }
    }
}