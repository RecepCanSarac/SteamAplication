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

    public int playerİndex;

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

            var playerList = GetPlayerList();
            
            var sortedPlayersWithClasses = SortPlayerAndClass(playerList);

            foreach (var pair in sortedPlayersWithClasses)
            {
                playerClasses.Add(pair.Item1);
                players.Add(pair.Item2);
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
            HandlePlayerMission();
        }
    }

    void HandlePlayerMission()
    {
        foreach (var player in Manager.GamePlayers)
        {
            player.mineHouse.isActivePlayer = false;
        }

        if (playerİndex >= players.Count)
        {
            players[playerİndex].mineHouse.isActivePlayer = true;
            playerİndex++;
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
    List<Tuple<string, PlayerObjectController>> SortPlayerAndClass(List<PlayerObjectController> players)
    {
        var enumOrder = Enum.GetValues(typeof(ClassType))
            .Cast<ClassType>()
            .Select(e => e.ToString())
            .ToList();

        var result = players
            .Where(player => enumOrder.Contains(player.syncedClassName) && !int.TryParse(player.syncedClassName, out _)) 
            .OrderBy(player => enumOrder.IndexOf(player.syncedClassName))
            .Select(player => new Tuple<string, PlayerObjectController>(player.syncedClassName, player))
            .ToList();

        return result;
    }

    List<PlayerObjectController> GetPlayerList()
    {
        List<PlayerObjectController> playerList = new List<PlayerObjectController>();

        foreach (var player in Manager.GamePlayers)
        {
            playerList.Add(player);
        }

        return playerList;
    }
}