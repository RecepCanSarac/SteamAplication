using Mirror;
using TMPro;
using UnityEngine.UI;

public class PlayerClass : NetworkBehaviour
{
    public ClassType Type;
    public TextMeshProUGUI ClassText;
    public SOClass Class;
    public string typeName;

    public void ClassTextMethod(ClassType type)
    {
        CmdClassName();
    }
    
    [Command]
    void CmdClassName()
    {
        typeName = Type.ToString();
    }
}
