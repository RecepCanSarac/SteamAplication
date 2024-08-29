using System;
using Mirror;
using TMPro;
using UnityEngine;

public class GameTimeline : NetworkBehaviour
{
    public TextMeshProUGUI timeText;

    [SyncVar(hook = nameof(OnSetTime))] public float time = 600.0f;

    void Update()
    {
        if(!isServer) return;
        
        time -= Time.deltaTime;

        SetTime();
    }

    public void SetTime()
    {
        CmdTime();
    }

    void OnSetTime(float oldValue, float newValue)
    {
        RpcTime(newValue);
    }

    [Command]
    void CmdTime()
    {
        RpcTime(time);
    }

    [ClientRpc]
    void RpcTime(float newValue)
    {
        int minutes = Mathf.FloorToInt(newValue / 60F);
        int seconds = Mathf.FloorToInt(newValue % 60F);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}