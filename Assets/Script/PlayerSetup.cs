using UnityEngine;
using Mirror;
using System;
using System.Linq;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Behaviour[] componentToDisable;
    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer)
        {
            for (int i = 0; i < componentToDisable.Length; i++)
            {
                componentToDisable[i].enabled = false;
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string id = GetComponent<NetworkIdentity>().netId.ToString();
    }

}
