using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TVScript : NetworkBehaviour
{
    public ClassGenerator playerClass;

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
       playerClass = GameObject.Find("GUIPanel").GetComponent<ClassGenerator>();


        if (playerClass.DataList.Count == 0)
        {
            foreach (var item in items)
            {
                item.gameObject.SetActive(false);
            }
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (i >= playerClass.DataList.Count || playerClass.DataList[i].Count == 0)
            {
                items[i].gameObject.SetActive(false);
            }
            else
            {
                items[i].gameObject.SetActive(true);
                names[i].text = playerClass.DataList[i].ClassName;
                numbers[i].text = playerClass.DataList[i].Count.ToString();
            }
        }
    }

}
