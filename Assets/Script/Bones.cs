using Mirror;
using UnityEngine;

public class Bones : NetworkBehaviour
{
    public GameObject res;
    public int bone, flesh;
    public void Resurrection()
    {

        GameManager.instance.RpcCreateUnit(3, transform.position);
        GameManager.instance.ServerDestroy(gameObject);
    }

    public void PickUped()
    {
        GameManager.instance.ServerDestroy(gameObject);
    }

}
