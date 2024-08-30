using System;
using Mirror;
using TMPro;
using UnityEngine;

public class GameTimeline : NetworkBehaviour
{
    public TextMeshProUGUI timeText;

    public TextMeshProUGUI messageText;

    public TextMeshProUGUI roundText;

    public GameObject gameCamera;

    #region SyncVar Variable

    [SyncVar(hook = nameof(OnCameraControll))] public bool _cameracontroll = true;

    [SyncVar(hook = nameof(OnSetTime))] public float time = 30.0f;

    [SyncVar(hook = nameof(OnSetRaound))] public int round = 0;

    [SyncVar(hook = nameof(OnPlayerIndex))]
    public int playerIndex = 0;

    [SyncVar(hook = nameof(OnOrderOfPlayer))]
    public PlayerObjectController orderOfPlayer;

    [SyncVar(hook = nameof(OnSendMessage))]
    public string message = string.Empty;
    

    #endregion
    
    #region Singleton

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

    #endregion

    private float currentTime;

    void Start()
    {
        gameCamera.SetActive(false);
        message = "Oyun başlıyor....";
        currentTime = time;
        GameTime();
    }

    void Update()
    {
        if (!isServer) return;

        time -= Time.deltaTime;

        if (time <= 0)
        {
            round++;
            message = Message();
            SetRound();
            SetPlayerIndex();
            GameTime();
            SetPlayer();
            time = currentTime;
            if (Manager.GamePlayers.Count - 1 > playerIndex) playerIndex++;
            else playerIndex = 0;
        }

        SetTime();
    }

    #region SetFunc

    public void SetTime()
    {
        CmdTime();
    }

    public void GameTime()
    {
        CmdSendMessage();
    }

    public void SetRound()
    {
        CmdSetRound();
    }

    public void SetPlayer()
    {
        orderOfPlayer = Manager.GamePlayers[playerIndex];
        foreach (PlayerObjectController players in Manager.GamePlayers)
        {
            players.SetCamera();
            players.SetMovement();
            SetCamera();
        }
        CmdSetPlayer();
    }

    public void SetPlayerIndex()
    {
        CmdSetPlayerIndex();
    }

    public void SetCamera()
    {
        _cameracontroll = !_cameracontroll;
        CmdSetCamera();
    }

    #endregion

    #region OnSyncvarFunc

    void OnSetTime(float oldValue, float newValue)
    {
        RpcTime(newValue);
    }

    void OnSendMessage(string oldValue, string newValue)
    {
        RpcMessage(newValue);
    }

    void OnSetRaound(int oldValue, int newValue)
    {
        RpcRound(newValue);
    }

    void OnOrderOfPlayer(PlayerObjectController oldValue, PlayerObjectController newValue)
    {
        RpcOrderOfPlayer(oldValue, newValue);
    }

    void OnPlayerIndex(int oldValue, int newValue)
    {
        RpcPlayerIndex(newValue);
    }

    void OnCameraControll(bool oldValue, bool newValue)
    {
        RpcCameraControll(newValue);
    }

    #endregion

    #region Command

    [Command]
    void CmdTime()
    {
        RpcTime(time);
    }

    [Command]
    void CmdSendMessage()
    {
        RpcMessage(message);
    }

    [Command]
    void CmdSetRound()
    {
        RpcRound(round);
    }

    [Command]
    void CmdSetPlayer()
    {
        RpcOrderOfPlayer(null, orderOfPlayer);
    }

    [Command]
    void CmdSetPlayerIndex()
    {
        RpcPlayerIndex(playerIndex);
    }

    [Command]
    void CmdSetCamera()
    {
        RpcCameraControll(_cameracontroll);
    }

    #endregion

    #region ClientRPC

    [ClientRpc]
    void RpcTime(float newValue)
    {
        timeText.text = newValue.ToString("00:00");
    }

    [ClientRpc]
    void RpcMessage(string newValue)
    {
        messageText.text = newValue;
    }

    [ClientRpc]
    void RpcRound(int newValue)
    {
        roundText.text = "Round : " + newValue;
    }

    [ClientRpc]
    void RpcOrderOfPlayer(PlayerObjectController oldValue, PlayerObjectController newValue)
    {
        if(oldValue != null) oldValue.ısOrderOf = false;
        newValue.ısOrderOf = true;
    }

    [ClientRpc]
    void RpcPlayerIndex(int index)
    {
        Debug.Log("player ındex : " + index.ToString());
    }

    [ClientRpc]
    void RpcCameraControll(bool newValue)
    {
        gameCamera.SetActive(newValue);
    }

    #endregion

    string Message()
    {
        string result = round switch
        {
            1 => "Oyun Başladı",
            2 => "Doktor Zamanı",
            _ => "round finished"
        };

        return result;
    }
}