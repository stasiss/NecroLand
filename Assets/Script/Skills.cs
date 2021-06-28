using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class Skills
{
    [Header("Global")]
    public int id;
    public string nameSkill;
    public string description;
    public int manaCost;
    public float timeCast;
    public float cdTime, tickCd;
    private SkillType skillType;
    private bool isUndead;
    [Header("Skills stats")]

    public bool isCiblable;
    private bool isUndeadCible, isHumansCible, isAura, isCanalisation, isPurification;
    public float range, sizeAoE;

    private Status[] status;

    private int damage, heal, moralChanger, probaProc;
    private TypeOfDamage typeDamage;

    [Header("Creation & Cost")]
    public UniteData unit;
    [HideInInspector] public int food, iron, wood;
    [HideInInspector] public int bone, flesh;

    public void Trigger(GameObject source, Vector3 target)
    {
        if (isUndead && GameManager.instance.mana < manaCost)
        {
            Debug.Log("pas assez de mana");
            return;
        }
        if (tickCd > Time.time)
        {
            Debug.Log("CD en cours");
            return;
        }
        if (range != 0 && Vector3.Distance(source.transform.position, target) > range)
        {
            Debug.Log("trop loin");
            return;
        }
        switch (skillType)
        {
            case SkillType.CreateUnitUD:
                if (source.GetComponent<Unit>().bone >= bone && source.GetComponent<Unit>().flesh >= flesh)
                {
                    GameManager.instance.CmdCostAndCreateUnitSkill(id, unit.Id, source.GetComponent<Unit>(), source.transform.position);
                }
                break;
            case SkillType.CreateUnitHU:
                if (GameManager.instance.food >= food && GameManager.instance.wood >= wood && GameManager.instance.iron >= iron)
                {
                    GameManager.instance.CmdCostAndCreateUnitSkill(id, unit.Id, source.GetComponent<Unit>(), source.transform.position);
                }
                break;
            case SkillType.RaiseZombie:
                if (GameObject.FindGameObjectsWithTag("Bones").Any(e => (e.transform.position - source.transform.position).magnitude < 5))
                {
                    List<GameObject> bones = GameObject.FindGameObjectsWithTag("Bones").Where(e => (e.transform.position - source.transform.position).magnitude < 5).ToList();
                    foreach (GameObject bone in bones)
                    {
                        bone.GetComponent<Bones>().Resurrection();
                    }
                }
                break;
            case SkillType.SkillAttack:
                List<Unit> targets = new List<Unit>();
                //Selecteion de la ou des cibles
                if (sizeAoE > 0)
                {
                    if (isHumansCible && isUndeadCible)
                        targets = (List<Unit>)GameObject.FindGameObjectsWithTag("Humans").Where(e => Vector3.Distance(e.transform.position, target) < sizeAoE).Select(e => e.GetComponent<Unit>()).ToList()
                                             .Union(GameObject.FindGameObjectsWithTag("Undead").Where(e => Vector3.Distance(e.transform.position, target) < sizeAoE).Select(e => e.GetComponent<Unit>()).ToList());

                    else if (isHumansCible)
                        targets = GameObject.FindGameObjectsWithTag("Humans").Where(e => Vector3.Distance(e.transform.position, target) < sizeAoE).Select(e => e.GetComponent<Unit>()).ToList();
                    else if (isUndeadCible)
                        targets = GameObject.FindGameObjectsWithTag("Undead").Where(e => Vector3.Distance(e.transform.position, target) < sizeAoE).Select(e => e.GetComponent<Unit>()).ToList();
                }
                else
                {
                    if (isHumansCible)
                    {
                        GameObject t = GameObject.FindGameObjectsWithTag("Humans").OrderBy(e => Vector3.Distance(e.transform.position, target)).FirstOrDefault(e => (e.transform.position - target).magnitude < 3);
                        if (t != null)
                            targets.Add(t.GetComponent<Unit>());
                    }
                    if (isUndeadCible)
                    {
                        GameObject t = GameObject.FindGameObjectsWithTag("Undead").OrderBy(e => Vector3.Distance(e.transform.position, target)).FirstOrDefault(e => (e.transform.position - target).magnitude < 3);
                        if (t != null)
                            targets.Add(t.GetComponent<Unit>());
                    }
                }
                if (targets.Count > 0)
                {
                    foreach (Unit u in targets)
                    {
                        if (probaProc != 0)
                        {
                            int alea = Random.Range(0, 100);
                            if (alea > probaProc)
                                continue;
                        }
                        if (status != null)
                        {
                            foreach (Status s in status)
                                u.ApplyStatus(s.id);
                        }
                        if (damage > 0)
                            u.TakeDamage(damage, typeDamage);
                        if (heal > 0)
                            u.Heal(heal);
                        if (moralChanger != 0)
                            u.MoralChanger(moralChanger);
                    }
                }
                else
                    return;

                break;
            default:
                break;
        }
        tickCd = Time.time + cdTime;
        if (isUndead)
            GameManager.instance.SetMana(-manaCost);
    }

    public void UseBDD(int skillID)
    {
        SkillBDD bdd = Resources.Load("SkillBDD") as SkillBDD;
        SkillData sd = bdd.Get(skillID);

        id = sd.Id;
        nameSkill = sd.Name;
        description = sd.Description;
        manaCost = sd.ManaCost;
        timeCast = sd.TimeCast;
        cdTime = sd.CdTime;
        skillType = sd.TypeSkill;
        isUndead = sd.IsUndead;
        isCiblable = sd.IsCiblageSkill;
        if (sd.TypeSkill == SkillType.CreateUnitUD)
        {
            unit = sd.Unit;
            bone = sd.CostBone;
            flesh = sd.CostFlesh;
        }
        if (sd.TypeSkill == SkillType.CreateUnitHU)
        {
            unit = sd.Unit;
            iron = sd.CostIron;
            wood = sd.CostWood;
            food = sd.CostFood;
        }
        if (sd.TypeSkill == SkillType.SkillAttack)
        {
            isUndeadCible = sd.IsUndeadCible;
            isHumansCible = sd.IsHumansCible;
            isAura = sd.IsAura;
            isCanalisation = sd.IsCanalisation;
            isPurification = sd.IsPurification;
            range = sd.Range;
            sizeAoE = sd.SizeAoE;

            if (sd.StatusArray.Count() > 0)
            {
                status = new Status[sd.StatusArray.Count()];
                for (int i = 0; i < sd.StatusArray.Count(); i++)
                {
                    Status s = new Status();
                    s.UseBDD(sd.StatusArray[i].Id);
                    status[i] = s;
                }
            }

            damage = sd.Damage;
            heal = sd.Heal;
            moralChanger = sd.MoralChanger;
            probaProc = sd.ProbaProc;
            typeDamage = sd.TypeDamage;
        }
    }
}
