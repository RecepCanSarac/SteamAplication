using System.Collections.Generic;
using Mirror;
using TMPro;

public class PlayerClass : NetworkBehaviour
{
    public TextMeshProUGUI[] texts;

    public List<string> className = new List<string>();

    private int indeks = 0;

    public void ShowClasses()
    {
        if (indeks > texts.Length) return;
        texts[indeks].text = className[indeks];
        indeks++;
    }
    //Add recep
    public void ShowClasses(List<string> clases)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = clases[i];
        }
    }
}