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
        for (int i = 0; i < playerClass.GetComponent<PlayerClass>().playerClasses.Count; i++)
        {
            if (playerClass.GetComponent<PlayerClass>().playerClasses.Count > 0)
            {
                names[i].text = playerClass.GetComponent<PlayerClass>().playerClasses[i].ClassName;
                numbers[i].text = playerClass.GetComponent<PlayerClass>().playerClasses[i].Count.ToString();
            }
            else
            {

                numbers[i].text = 0.ToString();
            }
        }

    }
}
