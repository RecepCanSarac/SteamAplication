using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassManagment : NetworkBehaviour
{
    public static ClassManagment Instance { get; private set; }

    public SyncList<SOClass> MangmentClass = new SyncList<SOClass>();
    public SyncList<GameObject> playerClasses = new SyncList<GameObject>();

    private bool okey = true;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (isServer)
            {
                GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
                CMDAddPlayer(player);
                if (okey == true)
                {
                    AssignClassesToPlayers();
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CMDAddListClass(SOClass item)
    {
        MangmentClass.Add(item);
    }

    [ClientRpc]
    void RpcRequestAddClass(SOClass item)
    {
        if (isServer)
        {
            CMDAddListClass(item);
        }
    }

    [Command(requiresAuthority = false)]
    private void CMDAddPlayer(GameObject[] players)
    {
        foreach (var player in players)
        {
            if (!playerClasses.Contains(player))
            {
                playerClasses.Add(player);
            }
        }

        RpcUpdatePlayerClasses(players);
    }

    [ClientRpc]
    private void RpcUpdatePlayerClasses(GameObject[] players)
    {
        foreach (var player in players)
        {
            if (!playerClasses.Contains(player))
            {
                playerClasses.Add(player);
            }
        }
    }
    private void AssignClassesToPlayers()
    {
        List<int> assignedIndexes = new List<int>();
        foreach (var player in playerClasses)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, MangmentClass.Count);
            }
            while (assignedIndexes.Contains(randomIndex));

            assignedIndexes.Add(randomIndex);

            /*player.GetComponent<PlayerClass>().ClassTextMethod(MangmentClass[randomIndex].ClassType);

            player.GetComponent<PlayerClass>().Class = MangmentClass[randomIndex];*/
            okey = false;
        }
    }
}
