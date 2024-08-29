using System;
using Mirror;
using TMPro;
using UnityEngine;

public class GameTimeline : NetworkBehaviour
{
    public TextMeshProUGUI timeText;

    public TextMeshProUGUI messageText;
    
    public TextMeshProUGUI roundText;

    [SyncVar(hook = nameof(OnSetTime))] public float time = 30.0f;
    
    [SyncVar(hook = nameof(OnSetRaound))] public int round = 0;

    [SyncVar(hook = nameof(OnSendMessage))]
    public string message = string.Empty;

    private float currentTime;

    void Start()
    {
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
            GameTime();
            time = currentTime;
        }

        SetTime();
    }

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