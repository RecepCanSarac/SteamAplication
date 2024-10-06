using Mirror;
using System;

public class House : NetworkBehaviour
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
