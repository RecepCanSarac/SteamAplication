using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerClass : NetworkBehaviour
{

    public TextMeshProUGUI ClassText;
    public SOClass Class;

    [Command]
    public void ClassTextMethod(ClassType type)
    {
        ClassText.text = type.ToString();
    }
}
