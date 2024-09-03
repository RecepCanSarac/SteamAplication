using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TVScript : NetworkBehaviour
{
    public GameObject playerClass;

    public GameObject[] items;
    public TextMeshProUGUI[] names;
    public TextMeshProUGUI[] numbers;
    CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Update()
    {
        if (playerClass == null)
        {
            playerClass = Manager.GamePlayers[0].gameObject;
        }

        var playerClassComponent = playerClass.GetComponent<PlayerClass>();

        if (playerClassComponent.playerClasses.Count == 0)
        {
            foreach (var item in items)
            {
                item.gameObject.SetActive(false);
            }
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (i >= playerClassComponent.playerClasses.Count || playerClassComponent.playerClasses[i].Count == 0)
            {
                items[i].gameObject.SetActive(false);
            }
            else
            {
                items[i].gameObject.SetActive(true);
                names[i].text = playerClassComponent.playerClasses[i].ClassName;
                numbers[i].text = playerClassComponent.playerClasses[i].Count.ToString();
            }
        }
    }

}
