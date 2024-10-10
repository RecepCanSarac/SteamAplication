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
    
    public List<string> classes = new List<string>();

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
        if (isServer)
        {
            playerClasses.Clear();
            var classList = GetClassTypeList();
            var sortedList = SortByEnumOrder(classList);
            foreach (var playerClass in sortedList)
            {
                playerClasses.Add(playerClass);
                classes.Add(playerClass);
            }
        }
        
        // Listenin güncellenmesi istemcilerde tetiklenir
        playerClasses.Callback += OnPlayerClassListUpdated;
    }

    private void OnPlayerClassListUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        // Bu metod tüm istemcilerde tetiklenir ve güncellemeyi sağlar
        UpdatePlayerClassesUI();
    }

    // UI'yi güncelleyen metod
    private void UpdatePlayerClassesUI()
    {
        // playerClasses listesine göre UI güncellemesi yapılabilir
        // Örneğin: messageText.text = string.Join(", ", playerClasses);
    }

    List<string> SortByEnumOrder(List<string> inputList)
    {
        // Enum değerlerini al ve sıralı bir string dizisine çevir
        var enumOrder = Enum.GetValues(typeof(ClassType))
            .Cast<ClassType>()
            .Select(e => e.ToString())
            .ToList();

        // String listesini enum sırasına göre sırala
        return inputList
            .OrderBy(item => enumOrder.IndexOf(item)) // Enum sırasına göre sırala
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
}