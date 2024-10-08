using System.Collections.Generic;
using Mirror;

public class Vote : NetworkBehaviour
{
    [SyncVar] public int votesReceived;
    
    [SyncVar]
    private PlayerObjectController votedFor;

    public List<string> playerVotes;
}