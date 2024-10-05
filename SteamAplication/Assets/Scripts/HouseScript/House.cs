using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour, IHouseManager
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
        if (Enum.TryParse(PlayerObjectController.className, out ClassType classType))
        {
            PlayerObjectController.ActivetedHouse(classType);
        }
    }
}
