using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public enum ClassType
{
    trapperStarFish, //1
    detectiveAnglerFish, //2
    ConfessorOctopus, //3
    ArmoredCrab, // 4
    healerShrimp, //5
    ThiefMorayEel, //6
    SerialKillerSeaMonster, //7
    KillerShark //8
}

public class ClassGenerator : NetworkBehaviour
{
    public Transform Content;
    public GameObject classItemPrefab;
    public GameObject ClassPrefab;
    public Transform ListContent;
    public List<SOClass> classes = new List<SOClass>();

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
    private void Update()
    {
        SetList();
    }
    public void CreateClassItem(SOClass classData)
    {
        GameObject itemIns = Instantiate(classItemPrefab, Content);
        var classItem = itemIns.GetComponent<ClassItem>();
        classItem.Setup(classData.ClassName, classData.ClassType);
        classItem.userClass = classData;
    }

    public void UpdateStackedClasses(SOClass newClass)
    {
        ClassType classType = newClass.ClassType;

        List<ClassData> newDataList = new List<ClassData>(DataList);

        ClassData classData = newDataList.Find(cd => cd.ClassType == classType);

        if (classData == null)
        {
            classData = new ClassData
            {
                ClassName = newClass.ClassName,
                ClassType = classType,
                Count = 1
            };
            newDataList.Add(classData);
        }
        else
        {
            classData.Count++;
        }

        DataList = newDataList;

        if (!classStackCounts.ContainsKey(classType))
        {
            classStackCounts[classType] = 0;
            GameObject classIns = Instantiate(ClassPrefab, ListContent);
            var classItem = classIns.GetComponent<ClassItem>();
            classItem.Setup(newClass.ClassName, newClass.ClassType);
            classItem.userClass = newClass;
            spawnedClassItems[classType] = classIns;
        }

        UpdateStackCount(classType);
        SetList();
        //Manager.UpdatedClassPlayer(newClass.ClassName.ToString());
    }
    public void RemoveFromStackedClasses(SOClass classToRemove)
    {
        ClassType classType = classToRemove.ClassType;

        List<ClassData> newDataList = new List<ClassData>(DataList);

        ClassData classData = newDataList.Find(cd => cd.ClassType == classType);

        if (classData != null)
        {
            classData.Count--;
            if (classData.Count <= 0)
            {
                newDataList.Remove(classData);

                if (classStackCounts.ContainsKey(classType))
                {
                    classStackCounts.Remove(classType);

                    if (spawnedClassItems.ContainsKey(classType))
                    {
                        GameObject classIns = spawnedClassItems[classType];
                        Destroy(classIns);
                        spawnedClassItems.Remove(classType);
                    }
                }
            }
        }
        UpdateStackCount(classType);
        DataList = newDataList;
        SetList();
        //Manager.UpdatedClassPlayer(classToRemove.ClassName.ToString());
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
        if (isLocalPlayer)
        {
            CmdSetList();
        }
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
}