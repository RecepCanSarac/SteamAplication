using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Test : MonoBehaviour
{
    public NetworkManager _manager;
    
    public void StartGame(string SceneName)
    {
        _manager.ServerChangeScene(SceneName);
    }
}
