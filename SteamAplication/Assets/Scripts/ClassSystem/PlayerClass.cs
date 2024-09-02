using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerClass : NetworkBehaviour
{
    public TextMeshProUGUI[] texts;
    public GameObject[] ClassItems;
    public List<string> className = new List<string>();

    private void Start()
    {
        ShowClasses();
    }

    public void ShowClasses()
    {
        //Change Recep burası güzel
        //Ben yaptım tabii güzel
        for (int i = 0; i < className.Count; i++)
        {
            ClassItems[i].SetActive(true);
            texts[i].gameObject.SetActive(true);
            texts[i].text = className[i];
        }
    }
    //Add recep
    public void ShowClasses(List<string> clases)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            ClassItems[i].SetActive(true);
            texts[i].gameObject.SetActive(true);
            texts[i].text = clases[i];
        }
    }
}