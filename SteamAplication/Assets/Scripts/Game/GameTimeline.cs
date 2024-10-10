using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class GameTimeline : NetworkBehaviour
{
    public TextMeshProUGUI timeText;

    public TextMeshProUGUI messageText;

    public TextMeshProUGUI roundText;

    public GameObject gameCamera;
    
    [SyncVar(hook = nameof(OnUpdatedList))]
    public List<string> playerClasses;

    private float time = 10;

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

    private void Update()
    {
        Round();
    }

    void Round()
    {
        time -= Time.deltaTime;
        timeText.text = time.ToString("00");

        if (time <= 0)
        {
            time = 10;
        }
    }

    private void Start()
    {
        PlayerObjectController player = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>();
        if (player.isLocalPlayer == true)
        {
            CmdSetList(playerClasses);
        }
    }

    void OnUpdatedList(List<string> oldValue, List<string> newValue)
    {
        RpcTargetList(newValue);
    }

    [Command(requiresAuthority = false)]
    void CmdSetList(List<string> newValue)
    {
        RpcTargetList(newValue);
    }

    [ClientRpc]
    void RpcTargetList(List<string> newValue)
    {
        newValue = GetClassTypeList();
        List<string> shortedList = SortByEnumOrder(newValue);
        newValue = shortedList;
    }

    List<string> SortByEnumOrder(List<string> inputList)
    {
        var enumOrder = Enum.GetValues(typeof(ClassType))
            .Cast<ClassType>()
            .Select(e => e.ToString())
            .ToList();

        return inputList
            .Where(item => enumOrder.Contains(item))
            .OrderBy(item => enumOrder.IndexOf(item))
            .ToList();
    }
    
    List<string> GetClassTypeList()
    {
        foreach (var player in Manager.GamePlayers)
        {
            playerClasses.Add(player.syncedClassName);
        }

        return playerClasses;
    }
}

