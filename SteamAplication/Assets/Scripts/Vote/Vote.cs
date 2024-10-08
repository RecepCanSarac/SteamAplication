using System.Collections.Generic;
using Mirror;

public class Vote : NetworkBehaviour
{
    [SyncVar] public int votesReceived;
    
    private PlayerObjectController votedFor;
}