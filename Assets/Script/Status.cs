using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Status
{
    [Header("Global")]
    public int id;
    public string nameStatus;
    public string description;
    public bool isNegatif;
    public Unit unitAffect;
    [Header("Stats")]
    public bool isCanalisation;
    public float timeLife, tickLife;
    public bool isProc;
    public float timeProc, tickProc;
    public bool isAura;
    public int idStatusAura;
    public bool isSoloAura;
    public bool isTargetUndead;
    public bool isInvisible;
    public bool isLifeSteal;
    [Header("Valeurs")]
    public int speedProdValue;
    public int speedMoveValue;
    public int speedAttaque;
    public int defCont;
    public int defTran;
    public int defMagi;
    public int boostDamage;
    public int heal;
    public int damage;
    public int damageAround;
    public TypeOfDamage typeDamage;
    public int moralModificateur;


    // Update is called once per frame
    public void VerifStatus()
    {
        if (isProc && Time.time > tickProc)
        {
            tickProc = Time.time + timeProc;

            if (damage > 0)
            {
                if (isLifeSteal)
                    unitAffect.LifeSteal(damage, typeDamage, GameObject.Find("Necro").GetComponent<Unit>());
                else
                    unitAffect.TakeDamage(damage, typeDamage);
            }
            if (heal > 0)
                unitAffect.Heal(heal);

            if (isAura)
            {
                Unit[] targets = isTargetUndead ? GameObject.FindGameObjectsWithTag("Undead").Where(e => (e.transform.position - unitAffect.transform.position).magnitude < 8).Select(e => e.GetComponent<Unit>()).ToArray() :
                                            GameObject.FindGameObjectsWithTag("Humans").Where(e => (e.transform.position - unitAffect.transform.position).magnitude < 8).Select(e => e.GetComponent<Unit>()).ToArray();
                foreach (Unit unitInAura in targets)
                {
                    if (damage > 0)
                    {
                        if (isLifeSteal)
                            unitInAura.LifeSteal(damage, typeDamage, unitAffect);
                        else
                            unitInAura.TakeDamage(damage, typeDamage);
                    }
                    if(moralModificateur != 0)
                        unitInAura.MoralChanger(moralModificateur);
                }

            }
        }
        if (tickLife != 0 && Time.time > tickLife)
        {
            unitAffect.SuppressionStatus(this);
        }
    }
    public void UseBDD(int statusID)
    {
        StatusBDD bdd = Resources.Load("StatusBDD") as StatusBDD;
        StatusData sd = bdd.Get(statusID);

        id = sd.Id;
        nameStatus = sd.Name;
        description = sd.Description;
        isNegatif = sd.IsNegatif;
        isCanalisation = sd.IsCanalisation;
        if (sd.TimerLife != 0)
        {
            timeLife = sd.TimerLife;
            tickLife = Time.time + timeLife;
        }
        if (sd.IsProc)
        {
            isProc = sd.IsProc;
            timeProc = sd.TimerProc;
            tickProc = Time.time;
        }
        isAura = sd.IsAura;
        isSoloAura = sd.IsSoloAura;
        idStatusAura = sd.IdStatusAura;
        isTargetUndead = sd.IsTargetUndead;
        speedProdValue = sd.SpeedProdValue;
        speedMoveValue = sd.SpeedMoveValue;
        speedAttaque = sd.SpeedAttaque;
        defCont = sd.DefContandant;
        defTran = sd.DefTranchant;
        defMagi = sd.DefMagique;
        boostDamage = sd.BoostDamage;
        isLifeSteal = sd.IsLifeSteal;
        damage = sd.Damage;
        heal = sd.Heal;
        damageAround = sd.DamageAround;
        typeDamage = sd.TypeDamage;
        moralModificateur = sd.MoralModificateur;
        isInvisible = sd.IsInvisible;

    }
}
