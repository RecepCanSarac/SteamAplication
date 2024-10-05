using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class House : NetworkBehaviour
{
    public bool isLelect { get; set; }
    public PlayerObjectController PlayerObjectController { get; set; }


    private void Update()
    {
        if (isLelect)
        {
            isActiveHouse();
        }
    }
    public void isActiveHouse()
    {
        if (Enum.TryParse(PlayerObjectController.className, out ClassType type))
        {
            PlayerObjectController.ActivetedHouse(type);
        }
    }
}
