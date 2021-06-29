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
        SpawnGameObject();
        GameManager.instance.ServerDestroy(gameObject);
    }

    public void PickUped()
    {
        GameManager.instance.ServerDestroy(gameObject);
    }

    [Server]
    public void SpawnGameObject()
    {
        GameObject go = Instantiate(res, transform.position, Quaternion.identity);
        Unit newUnit = go.GetComponentInChildren<Unit>();
        int stat = GameManager.instance.statZombi;
        newUnit.maxHealth = healthMax;
        newUnit.damage = damage * stat / 100;
        newUnit.defCont = armureCont + 1;
        newUnit.defTran = armureTran + 1;
        newUnit.defMagi = armureMagi + 1;
        newUnit.cdAttaque = timeAttack * 100f / stat;
        newUnit.SetSpeed(speed * stat / 100f);
        newUnit.typeOfDamage = typeOfDamage;
        NetworkServer.Spawn(go);
    }
}
