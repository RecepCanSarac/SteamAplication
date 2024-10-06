using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class House : NetworkBehaviour
{
    public bool isSelect { get; set; }
    public PlayerObjectController PlayerObjectController { get; set; }


    private void Update()
    {
        if (isSelect)
        {
            isActiveHouse();
        }
    }
    public void isActiveHouse()
    {
        if (Enum.TryParse(PlayerObjectController.className, out ClassType type))
        {
            if (PlayerObjectController.isLocalPlayer)
            {
                PlayerObjectController.ActivetedHouse(type);
            }
        }
    }

}
