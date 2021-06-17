using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CastleScript : NetworkBehaviour
{
    public int maxHealth;
    [HideInInspector] public bool isAlive = true;
    [SyncVar(hook = nameof(HealthChanged))]
    [HideInInspector]public int currentHealth;

    void Start()
    {
        if (isServer)
            currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        if (isServer)
            currentHealth -= damage;
        else
            CmdTakeDamage(damage);
    }
    /// <summary>
    /// fais remonter le moral de toutes les unités militaires autour du village
    /// </summary>
    public void ModifVillageMoralGlobal()
    {
        foreach (Unit u in GameManager.instance.armeeHuman.Where(e => Vector3.Distance(e.transform.position, transform.position) < 8 && e.GetComponent<Unit>() != null).Select(e => e.GetComponent<Unit>()))
        {
            u.MoralChanger(3);
        }
    }
    [Command]
    public void CmdTakeDamage(int damage)
    {
        TakeDamage(damage);
    }
    public void HealthChanged(int oldValue, int newValue)
    {
        if (newValue <= 0 && isAlive)
        {
            isAlive = false;
            GameManager.instance.ServerDestroy(gameObject);
        }
    }
}
