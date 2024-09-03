using System;
using System.Collections.Generic;
using Mirror;
using Org.BouncyCastle.Asn1.Mozilla;
using TMPro;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    public TextMeshProUGUI[] texts;
    public GameObject[] ClassItems;
    public TextMeshProUGUI[] numbers;
    public List<ClassData> playerClasses = new List<ClassData>();
    private void Start()
    {
        ShowClasses();
    }

    public void ShowClasses()
    {
        //for (int i = 0; i < playerClasses.Count; i++)
        //{
        //    if (i < ClassItems.Length && i < texts.Length)
        //    {
        //        ClassItems[i].SetActive(true);
        //        texts[i].gameObject.SetActive(true);
        //        texts[i].text = playerClasses[i].ClassName;
        //        numbers[i].text = playerClasses[i].Count.ToString();
        //    }
        //}
    }

    public void ShowClasses(List<ClassData> classes)
    {
        playerClasses = classes;
        ShowClasses();
    }
}


[Serializable]
public class ClassData
{
    public ClassType ClassType;
    public string ClassName;
    public int Count;

    public ClassData() { }

    public ClassData(ClassType ClassType, string ClassName, int _count)
    {
        this.ClassType = ClassType;
        this.ClassName = ClassName;
        this.Count = _count;
    }
}