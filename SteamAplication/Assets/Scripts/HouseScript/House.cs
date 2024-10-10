using Mirror;
using System;
using UnityEngine;

public class House : MonoBehaviour
{
    public bool isSelect { get; set; }
    public PlayerObjectController PlayerObjectController { get; set; }

    public string type;

    public bool isActivePlayer = false;

    private void OnMouseDown()
    {
        if (isSelect)
        {
            isActiveHouse();
        }
    }

    public void isActiveHouse()
    {
        if (type != PlayerObjectController.syncedClassName)
        {
            PlayerObjectController.ActivetedHouse(type);
        }
    }
}