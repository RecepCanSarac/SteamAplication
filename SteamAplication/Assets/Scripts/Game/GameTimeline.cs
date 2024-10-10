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
    public List<PlayerObjectController> playerObjList = new List<PlayerObjectController>();
    public List<string> classes = new List<string>();

    public int startTime = 5;
    public bool startGame = false;
    private float time = 10;

    private DayTime _dayTime;
    public int orderNumber = 0;

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

            // Hem sınıf listesini hem de oyuncu nesnelerini getiriyoruz
            var classList = GetClassTypeList();

            SortPlayersAndClasses(classList, out var sortedPlayerObjList);

            foreach (var playerClass in classList)
            {
                playerClasses.Add(playerClass);
                classes.Add(playerClass);
            }

            playerObjList = sortedPlayerObjList; // Oyuncu nesneleri sıralanmış olarak atanıyor
        }

        playerClasses.Callback += OnPlayerClassListUpdated;
        orderNumber = playerClasses.Count;
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

        if (time <= 0 && !startGame)
        {
            SetPlayerCamera(true);
            time = 10;
            startGame = true;
        }

        if (startGame && time <= 0 && orderNumber >= 0)
        {
            time = 10;
            orderNumber--;
        }
    }

    void OnPlayerClassListUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        UpdatePlayerClassesUI();
    }

    void UpdatePlayerClassesUI()
    {
        // Sınıflar UI'ye güncellenecek
    }

    // Sıralama işlemi: hem sınıf listesini hem de oyuncu objelerini sıralar
    void SortPlayersAndClasses(List<string> sortedClassList, out List<PlayerObjectController> sortedPlayerObjList)
    {
        var enumOrder = Enum.GetValues(typeof(ClassType))
            .Cast<ClassType>()
            .Select(e => e.ToString())
            .ToList();

        var combinedList = playerObjList
            .Where(player => enumOrder.Contains(player.syncedClassName))
            .OrderBy(player => enumOrder.IndexOf(player.syncedClassName))
            .ToList();

        sortedClassList = combinedList
            .Select(player => player.syncedClassName)
            .ToList();

        sortedPlayerObjList = combinedList;
    }

    // Hem sınıf isimlerini hem de playerObjList'i doldurur
    List<string> GetClassTypeList()
    {
        List<string> classTypeList = new List<string>();

        foreach (var player in Manager.GamePlayers)
        {
            classTypeList.Add(player.syncedClassName);
            playerObjList.Add(player); // Oyuncu nesnelerini listeye ekliyoruz
        }

        return classTypeList;
    }
}
