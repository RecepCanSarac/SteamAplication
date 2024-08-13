using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ClassType
{
    Doctor, Detective, Seer, Armored, Confessor, Thief, Trapper, Buffoon, Lookout, Killer
}

public class ClassGenerator : MonoBehaviour
{
    public Transform Content;
    public GameObject classItem;
    public List<SOClass> classes = new List<SOClass>();
    public List<SOClass> Userclasses = new List<SOClass>();
    private Dictionary<ClassType, int> classStackCounts = new Dictionary<ClassType, int>();
    private Dictionary<ClassType, GameObject> spawnedClassItems = new Dictionary<ClassType, GameObject>();

    public GameObject Class;
    public Transform ListContent;
    private int index = 0;

    private void Start()
    {
        for (int i = 0; i < classes.Count; i++)
        {
            GameObject itemIns = Instantiate(classItem, Content);
            itemIns.GetComponent<ClassItem>().Setup(classes[i].ClassName, classes[i].ClassIcon, classes[i].ClassType);
            itemIns.GetComponent<ClassItem>().userClass = classes[i];
        }
    }

    public void UpdateStackedClasses(SOClass newClass)
    {
        ClassType classType = newClass.ClassType;

        if (!classStackCounts.ContainsKey(classType))
        {
            classStackCounts[classType] = 0;
            GameObject classIns = Instantiate(Class, ListContent);
            classIns.GetComponent<ClassItem>().Setup(newClass.ClassName, newClass.ClassIcon, newClass.ClassType);
            spawnedClassItems[classType] = classIns;
        }

        classStackCounts[classType]++;
        UpdateStackCount(classType);
    }

    private void UpdateStackCount(ClassType classType)
    {
        if (classStackCounts.ContainsKey(classType))
        {
            TextMeshProUGUI text = spawnedClassItems[classType].GetComponent<ClassItem>().NumberText;
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
