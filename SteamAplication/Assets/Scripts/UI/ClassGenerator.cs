using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public enum ClassType
{
    Doctor, Detective, Seer, Armored, Confessor, Thief, Trapper, Buffoon, Lookout, Killer
}

public class ClassGenerator : NetworkBehaviour
{
    public Transform Content;
    public GameObject classItemPrefab;
    public List<SOClass> classes = new List<SOClass>();
    public List<SOClass> Userclasses = new List<SOClass>();
    private Dictionary<ClassType, int> classStackCounts = new Dictionary<ClassType, int>();
    private Dictionary<ClassType, GameObject> spawnedClassItems = new Dictionary<ClassType, GameObject>();

    public GameObject ClassPrefab;
    public Transform ListContent;

    private void Start()
    {
        for (int i = 0; i < classes.Count; i++)
        {
            CreateClassItem(classes[i]);
        }
    }

    [Server]
    private void CreateClassItem(SOClass classData)
    {
        GameObject itemIns = Instantiate(classItemPrefab, Content);
        var classItem = itemIns.GetComponent<ClassItem>();
        classItem.Setup(classData.ClassName, classData.ClassIcon, classData.ClassType);
        classItem.userClass = classData;
        NetworkServer.Spawn(itemIns);
    }

    [Server]
    public void UpdateStackedClasses(SOClass newClass)
    {
        ClassType classType = newClass.ClassType;

        if (!classStackCounts.ContainsKey(classType))
        {
            classStackCounts[classType] = 0;

            GameObject classIns = Instantiate(ClassPrefab, ListContent);
            var classItem = classIns.GetComponent<ClassItem>();
            classItem.Setup(newClass.ClassName, newClass.ClassIcon, newClass.ClassType);
            NetworkServer.Spawn(classIns); 
            spawnedClassItems[classType] = classIns;
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
}
