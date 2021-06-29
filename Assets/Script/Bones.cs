using Mirror;
using UnityEngine;

public class Bones : NetworkBehaviour
{
    public GameObject res;
    public int bone, flesh;
    public int healthMax, damage, armureCont, armureTran, armureMagi;
    public float timeAttack,speed;
    public TypeOfDamage typeOfDamage;
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
