using Mirror;
using System;
using UnityEngine;

public class House : MonoBehaviour
{
    public bool isSelect { get; set; }
    public PlayerObjectController PlayerObjectController { get; set; }

    public ClassType type;

    private void OnMouseDown()
    {
        if (isSelect)
        {
            isActiveHouse();
        }
    }

    public void isActiveHouse()
    {
        if (type != PlayerObjectController.type)
        {
            PlayerObjectController.ActivetedHouse(type);
        }
    }
}