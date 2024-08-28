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
        if(indeks > 3) return;
        texts[indeks].text = className[indeks];
        indeks++;
    }
}