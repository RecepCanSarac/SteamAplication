using System;
using Mirror;
using UnityEngine;

public class GamePlayerSpawn : NetworkBehaviour
{
    #region Singleton

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }

            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    #endregion

    public Transform[] points;

    void Start()
    {
        for (int i = 0; i < Manager.GamePlayers.Count; i++)
        {
            Vector3 offset = new Vector3(points[i].position.x, 1.6f, points[i].position.z);

            Manager.GamePlayers[i].transform.position = offset;
        }
    }
}