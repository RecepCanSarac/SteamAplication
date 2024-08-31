using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public enum ClassType
{
    Doctor,
    Detective,
    Seer,
    Armored,
    Confessor,
    Thief,
    Trapper,
    Buffoon,
    Lookout,
    Killer
}

public class ClassGenerator : NetworkBehaviour
{
    public Transform Content;
    public GameObject classItemPrefab;

    public GameObject currentObject;

    public List<SOClass> classes = new List<SOClass>();

    public List<string> Userclasses = new List<string>();

    [SyncVar(hook = nameof(OnListChanged))]
    public List<string> Currentclasses = new List<string>();

    public static ClassGenerator Instance;

    private void Awake() => Instance = this;

    private Dictionary<ClassType, int> classStackCounts = new Dictionary<ClassType, int>();
    private Dictionary<ClassType, GameObject> spawnedClassItems = new Dictionary<ClassType, GameObject>();

    public GameObject ClassPrefab;
    public Transform ListContent;

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

    private void Start()
    {
        for (int i = 0; i < classes.Count; i++)
        {
            CreateClassItem(classes[i]);
        }
    }

    public void CreateClassItem(SOClass classData)
    {
        GameObject itemIns = Instantiate(classItemPrefab, Content);
        var classItem = itemIns.GetComponent<ClassItem>();
        classItem.Setup(classData.ClassName, classData.ClassType);
        classItem.userClass = classData;
        //NetworkServer.Spawn(itemIns);
    }

    public void UpdateStackedClasses(SOClass newClass)
    {
        ClassType classType = newClass.ClassType;

        if (!classStackCounts.ContainsKey(classType))
        {
            classStackCounts[classType] = 0;
            GameObject classIns = Instantiate(ClassPrefab, ListContent);
            var classItem = classIns.GetComponent<ClassItem>();
            classItem.Setup(newClass.ClassName, newClass.ClassType);
            NetworkServer.Spawn(classIns);
            spawnedClassItems[classType] = classIns;

            List<string> updatedClasses = new List<string>(Currentclasses);
            updatedClasses.Add(newClass.ClassName.ToString());
            Currentclasses = updatedClasses;

            Manager.UpdatedClassPlayer(newClass.ClassName.ToString());

            SetList();
        }
        else
        {
            classStackCounts[classType]++;

            UpdateStackCount(classType);
        }
    }

    private void UpdateStackCount(ClassType classType)
    {
        if (classStackCounts.ContainsKey(classType))
        {
            var classItem = spawnedClassItems[classType];
            if (classItem != null)
            {
                TextMeshProUGUI text = classItem.GetComponent<ClassItem>().NumberText;
                if (classStackCounts[classType] > 1)
                {
                    text.text = classStackCounts[classType].ToString();
                }
                else
                {
                    text.text = "1";
                }
            }
        }
    }
    //Add Recep
    public void GetCurrentList()
    {
        if (isServer)
        {
            GetList(Currentclasses);
        }
    }

    public void SetList()
    {
        CmdSetList();
    }
    void OnListChanged(List<string> oldValue, List<string> newValue)
    {
        if (isServer)
        {
            RpcSetList(newValue);
        }
    }
    
    [Command]
    void GetList(List<string> clases)
    {
        GetRPCList(clases);
    }
    
    [Command]
    void CmdSetList()
    {
        RpcSetList(Currentclasses);
    }

    [ClientRpc]
    void RpcSetList(List<string> newValue)
    {
        for (int i = 0; i < Manager.GamePlayers.Count; i++)
        {
            PlayerClass playerClass = Manager.GamePlayers[i].GetComponent<PlayerClass>();

            playerClass.className.Clear();
            playerClass.className = newValue;

            playerClass.ShowClasses();
        }

        Userclasses = newValue;
    }
    //Add recep
    [ClientRpc]
    void GetRPCList(List<string> newValue)
    {
        for (int i = 0; i < Manager.GamePlayers.Count; i++)
        {
            PlayerClass playerClass = Manager.GamePlayers[i].GetComponent<PlayerClass>();

            playerClass.className = newValue;

            playerClass.ShowClasses(newValue);
        }

        Userclasses = newValue;
    }
}