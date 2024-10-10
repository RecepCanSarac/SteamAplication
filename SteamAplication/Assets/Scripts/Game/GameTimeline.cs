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

    public SyncList<string> playerClasses = new SyncList<string>();

    public SyncList<PlayerObjectController> players = new SyncList<PlayerObjectController>();

    public int startTime = 5;
    public bool startGame = false;
    private float time = 10;

    private DayTime _dayTime;

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

    void Start()
    {
        _dayTime = GetComponent<DayTime>();

        SetPlayerCamera(false);

        time = startTime;

        if (isServer)
        {
            playerClasses.Clear();
            players.Clear();

            var classList = GetClassTypeList();

            var playerList = GetPlayerList();
            
            var sortedPlayers = SortPlayerObject(playerList); 

            var sortedList = SortByEnumOrder(classList);

            foreach (var player in sortedPlayers)
            {
                players.Add(player);
            }

            foreach (var playerClass in sortedList)
            {
                playerClasses.Add(playerClass);
            }
        }

        playerClasses.Callback += OnPlayerClassListUpdated;
        players.Callback += OnPlayerListUpdated;
    }

    void Update()
    {
        Round();
    }

    void SetPlayerCamera(bool isActive)
    {
        gameCamera.SetActive(isActive);

        _dayTime.HandleInput(!isActive);
    }

    void Round()
    {
        time -= Time.deltaTime;
        timeText.text = time.ToString("00");

        if (time <= 0 && startGame == false)
        {
            SetPlayerCamera(true);
            time = 10;
            startGame = true;
        }

        if (startGame == true && time <= 0)
        {
            time = 10;
        }
    }

    void OnPlayerClassListUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        UpdatePlayerClassesUI();
    }

    void OnPlayerListUpdated(SyncList<PlayerObjectController>.Operation op, int index, PlayerObjectController oldItem, PlayerObjectController newItem)
    {
        UpdatePlayerList();
    }

    void UpdatePlayerClassesUI()
    {
        //Update string list
    }

    void UpdatePlayerList()
    {
        //update Player list
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

    List<PlayerObjectController> SortPlayerObject(List<PlayerObjectController> inputList)
    {
        var enumOrder = Enum.GetValues(typeof(ClassType))
            .Cast<ClassType>()
            .Select(e => e.ToString())
            .ToList();

        return inputList
            .Where(player => enumOrder.Contains(player.syncedClassName))
            .OrderBy(player => enumOrder.IndexOf(player.syncedClassName))
            .ToList();
    }

    List<string> GetClassTypeList()
    {
        List<string> classTypeList = new List<string>();

        foreach (var player in Manager.GamePlayers)
        {
            classTypeList.Add(player.syncedClassName);
        }

        return classTypeList;
    }
    
    List<PlayerObjectController> GetPlayerList()
    {
        List<PlayerObjectController> classTypeList = new List<PlayerObjectController>();

        foreach (var player in Manager.GamePlayers)
        {
            classTypeList.Add(player);
        }

        return classTypeList;
    }
}