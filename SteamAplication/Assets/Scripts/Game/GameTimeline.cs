using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class GameTimeline : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    public TextMeshProUGUI messageText;

    public TextMeshProUGUI roundText;

    public GameObject gameCamera;

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
        playerClasses = GetClassTypeList();
        List<string> shortedList = SortByEnumOrder(playerClasses);
        playerClasses = shortedList;
    }

    public List<string> SortByEnumOrder(List<string> inputList)
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
    
    public List<string> GetClassTypeList()
    {
        foreach (var player in Manager.GamePlayers)
        {
            playerClasses.Add(player.syncedClassName);
        }

        return playerClasses;
    }
}

