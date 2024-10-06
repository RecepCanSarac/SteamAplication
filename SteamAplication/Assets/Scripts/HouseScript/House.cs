using Mirror;
using System;
using UnityEngine;

public class House : MonoBehaviour
{
    public bool isSelect { get; set; }

    public bool isLocalPlayer = false;
    public PlayerObjectController PlayerObjectController { get; set; }

    public ClassType type;

    private void OnMouseDown()
    {
        if (isLocalPlayer)
        {
            if (isSelect)
            {
                isActiveHouse();
            }
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
