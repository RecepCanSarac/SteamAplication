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
    public GameObject ClassPrefab;
    public Transform ListContent;
    public List<SOClass> classes = new List<SOClass>();
    public List<string> Userclasses = new List<string>();

    [SyncVar(hook = nameof(OnListChanged))]
    public List<ClassData> DataList = new List<ClassData>();

    public static ClassGenerator Instance;

    private void Awake() => Instance = this;

    private Dictionary<ClassType, int> classStackCounts = new Dictionary<ClassType, int>();
    private Dictionary<ClassType, GameObject> spawnedClassItems = new Dictionary<ClassType, GameObject>();

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

        ClassData classData = DataList.Find(cd => cd.ClassType == classType);

        if (classData == null)
        {
            classData = new ClassData
            {
                ClassName = newClass.ClassName,
                ClassType = classType,
                Count = 1
            };
            DataList.Add(classData);
        }
        else
        {
            classData.Count++;
        }

        if (!classStackCounts.ContainsKey(classType))
        {
            classStackCounts[classType] = 0;
            GameObject classIns = Instantiate(ClassPrefab, ListContent);
            var classItem = classIns.GetComponent<ClassItem>();
            classItem.Setup(newClass.ClassName, newClass.ClassType);
            spawnedClassItems[classType] = classIns;
        }

        UpdateStackCount(classType);
        SetList();
        Manager.UpdatedClassPlayer(newClass.ClassName.ToString());
    }

    private void UpdateStackCount(ClassType classType)
    {
        if (classStackCounts.ContainsKey(classType))
        {
            var classItem = spawnedClassItems[classType];
            if (classItem != null)
            {
                TextMeshProUGUI text = classItem.GetComponent<ClassItem>().NumberText;
                int count = DataList.Find(cd => cd.ClassType == classType).Count;
                text.text = count.ToString();
            }
        }
    }

    public void SetList()
    {
        if(isServer)
            CmdSetList();
    }

    void OnListChanged(List<ClassData> oldValue, List<ClassData> newValue)
    {
        if (isServer)
        {
            RpcSetList(newValue);
        }
    }

    [Command]
    void CmdSetList()
    {
        RpcSetList(DataList);
    }

    [ClientRpc]
    void RpcSetList(List<ClassData> newValue)
    {

        for (int i = 0; i < Manager.GamePlayers.Count; i++)
        {
            PlayerClass playerClass = Manager.GamePlayers[i].GetComponent<PlayerClass>();
            playerClass.ShowClasses();
            playerClass.ShowClasses(newValue);
        }
    }

    [ClientRpc]
    void GetRPCList(List<ClassData> newValue)
    {
        for (int i = 0; i < Manager.GamePlayers.Count; i++)
        {
            PlayerClass playerClass = Manager.GamePlayers[i].GetComponent<PlayerClass>();
            playerClass.ShowClasses(newValue);
        }
        Userclasses = newValue.ConvertAll(cd => cd.ClassName);
    }
}