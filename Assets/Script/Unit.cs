using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Unit : NetworkBehaviour
{
    public int id;
    public NavMeshAgent agent;
    [HideInInspector] public bool isUndead;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public bool isAlive = true;
    public Image healthBarre;
    [SyncVar(hook = nameof(HealthChanged))]
    [HideInInspector] public int currentHealth;
    [SyncVar] [HideInInspector] public StatMoral currentEtat;
    [SyncVar] public int currentMoral, middleMoral, maxMoral;
    [HideInInspector] [SyncVar] public int moralSeuilFuite, moralSeuilBas, moralSeuilHaut;
    [HideInInspector] [SyncVar] public int damage;
    [HideInInspector] public TypeOfDamage typeOfDamage;
    [SyncVar] public int defCont, defTran, defMagi;
    [HideInInspector] [SyncVar] public float speed, speedBase;
    [HideInInspector] [SyncVar] public float range;
    [HideInInspector] [SyncVar] public float fieldOfView;
    [HideInInspector] [SyncVar] public float sizeZoneOfAura;
    [HideInInspector] public float rangeToAggro;
    [HideInInspector] [SyncVar] public float cdAttaque, cdAttaqueBase;
    public SyncList<Skills> skillsList = new SyncList<Skills>();
    public SyncList<Status> statusList = new SyncList<Status>();
    private float tickAttaque;
    private float tickVerifStatus;
    [HideInInspector] public int idCorpse;
    public GameObject corpse;
    [HideInInspector] public int probaDropCorpse;
    [HideInInspector] public int corpseBone, corpseFlesh;
    public GameObject projectile;
    public Material[] selectedMaterial;
    public Material[] normalMaterial;
    [HideInInspector] public float consomptionFood;
    private float manaReserve;
    [Header("Transport")]
    [HideInInspector] public bool isTransport;
    [SyncVar] public GameObject targetVillage;
    private bool isReturn;
    [HideInInspector] public int stockMax;
    [HideInInspector] public int food, iron, wood;
    [HideInInspector] public int bone, flesh;
    [HideInInspector] public int[] orders = new int[4];
    [HideInInspector] public int colon;
    public bool isHero;

    [Header("Paysans")]

    [HideInInspector] public bool isRecolteur;

    [HideInInspector] public bool isCorpseRecolteur;
    [HideInInspector] [SyncVar] public GameObject village;
    [HideInInspector] [SyncVar] public GameObject ressZone;
    private bool isCharged;

    [Header("Animation")]
    private bool isMoving;
    private ActionUnite currentAction;

    private GameObject target;
    [SyncVar] public Vector3 targetMove;
    [SyncVar] public Vector3 targetFinishMove;

    private void Awake()
    {
        UseBDD();
    }
    // Start is called before the first frame update
    void Start()
    {
        sizeZoneOfAura = 8;
        cdAttaqueBase = cdAttaque;
        speedBase = speed;
        isUndead = CompareTag("Undead");
        if (isServer)
        {
            InitHeal();
        }
        if (isTransport)
            GoStartVillage();
        //transform.position = VectorZeroY(transform.position);
    }
    public void GoStartVillage()
    {
        currentAction = ActionUnite.idle;
        if (isServer)
        {
            if (isReturn)
            {
                isReturn = false;
                agent.destination = VectorZeroY(targetVillage.transform.position);
                RotationToUnit();
            }
            else
            {
                isReturn = true;
                agent.destination = VectorZeroY(GameObject.Find("Castle").transform.position);
                RotationToUnit();
            }
        }
        else
            CmdGoStartVillage();
    }
    [Command]
    public void CmdGoStartVillage()
    {
        GoStartVillage();
    }

    public List<string> Stats()
    {
        List<string> result = new List<string>();
        result.Add(gameObject.name);
        result.Add($"{currentHealth}/{maxHealth}PV");
        if (CompareTag("Humans"))
            result.Add($"{currentMoral}/{maxMoral} {currentEtat}");
        if (cdAttaque != 0)
        {
            result.Add($"{damage} Damages - {range} PO");
            result.Add($"{String.Format("{0:0.00}", 1 / cdAttaque)} attaque/sec");
        }
        else
            result.Add("Attaque impossible");
        result.Add($"{defCont}AC {defTran}AT {defMagi}AM");
        result.Add($"{speed} vitesse");
        if (stockMax > 0)
        {
            if (wood + iron + food > 0)
            {
                string stockDisplay = "";
                if (wood > 0)
                    stockDisplay += $" {wood}wood";
                if (food > 0)
                    stockDisplay += $" {food}food";
                if (iron > 0)
                    stockDisplay += $" {iron}iron";
                result.Add($"{stockDisplay} / {stockMax} Stock");
            }
            else
                result.Add($"0 / {stockMax} Stock");
        }
        if (isCorpseRecolteur)
            result.Add($"{bone}Os / {flesh}Chair");
        return result;
    }
    public void SuppressionStatus(Status status)
    {
        speed = speedBase;
        cdAttaque = cdAttaqueBase;
        Debug.Log(defCont);
        defCont -= status.defCont;
        Debug.Log(defCont);
        defTran -= status.defTran;
        defMagi -= status.defMagi;
        damage -= status.boostDamage;
        statusList.Remove(status);
    }
    [Command(requiresAuthority = false)]
    public void CmdApplyStatus(int idStatus)
    {
        ApplyStatus(idStatus);
    }
    public void ApplyStatus(int idStatus)
    {
        if (isServer)
        {
            if (statusList.Any(s => s.id == idStatus))
            {
                Status status = statusList.First(s => s.id == idStatus);
                status.tickLife = status.timeLife + Time.time;
            }
            else
            {
                Status s = new Status();
                s.UseBDD(idStatus);
                statusList.Add(s);
                s.unitAffect = this;
                if (!s.isProc)
                {
                    speedBase = speed;
                    speed *= (100f + s.speedMoveValue) / 100f;
                    cdAttaqueBase = cdAttaque;
                    cdAttaque *= (100f + s.speedAttaque) / 100f;
                    defCont += s.defCont;
                    defTran += s.defTran;
                    defMagi += s.defMagi;
                    damage += s.boostDamage;
                }
            }
        }
        else
        {
            CmdApplyStatus(idStatus);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdChangeAction(ActionUnite act, Vector3 targetMouv)
    {
        ChangeAction(act, targetMouv);
    }
    public void ChangeAction(ActionUnite act, Vector3 targetMouv)
    {
        if (isServer)
        {
            if (act == ActionUnite.work)
            {
                currentAction = ActionUnite.mouv;
                if (isCharged)
                    TargetMove(village.transform.position);
                else
                    TargetMove(ressZone.transform.position);
                return;
            }
            currentAction = act;
            TargetMove(VectorZeroY(targetMouv));
            targetFinishMove = VectorZeroY(targetMouv);
            if (act == ActionUnite.mouv)
                target = null;
        }
        else
            CmdChangeAction(act, targetMouv);
    }
    public void TargetMove(Vector3 newMove)
    {
        if (isTransport || (CompareTag("Humans") && currentEtat == StatMoral.fleeing))
            return;
        if (isServer)
        {
            targetFinishMove = newMove;
            agent.destination = newMove;
            RotationToUnit();
        }
    }

    internal void MoralChanger(int moralChanger)
    {
        currentMoral = Mathf.Clamp(currentMoral + moralChanger, 0, maxMoral);
        if (currentEtat == StatMoral.fleeing && Vector3.Distance(transform.position, targetMove) > 5)
            return;
        if (currentMoral < moralSeuilFuite)
        {
            TargetMove(GameObject.Find("Castle").transform.position);
            currentEtat = StatMoral.fleeing;
        }
        else if (currentMoral < moralSeuilBas)
            currentEtat = StatMoral.scared;
        else if (currentMoral > moralSeuilHaut)
            currentEtat = StatMoral.determined;
        else
            currentEtat = StatMoral.normal;
    }

    [Command]
    public void CmdSetTargetMove(Vector3 newMove)
    {
        TargetMove(newMove);
    }
    // Update is called once per frame
    void Update()
    {
        if (statusList.Count > 0 && Time.time > tickVerifStatus)
        {
            tickVerifStatus = Time.time + 1;
            for (int i = statusList.Count - 1; i >= 0; i--)
            {
                if (statusList[i].isAura)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, sizeZoneOfAura);

                    foreach (Unit u in hitColliders.Where(e => e.GetComponentInParent<Unit>()).Select(e => e.GetComponentInParent<Unit>()))
                    {
                        if (statusList[i].idStatusAura != 0)
                            u.ApplyStatus(statusList[i].idStatusAura);
                        if (statusList[i].damage != 0)
                        {
                            if (statusList[i].isLifeSteal)
                                u.LifeSteal(statusList[i].damage, statusList[i].typeDamage, this);
                            else
                                u.TakeDamage(statusList[i].damage, statusList[i].typeDamage);
                        }
                        if (statusList[i].moralModificateur != 0)
                        {
                            u.MoralChanger(statusList[i].moralModificateur);
                        }
                    }
                }
                statusList[i].VerifStatus();
            }
        }
        if (currentAction != ActionUnite.mouv)
            target = FindTheCloseEnemy(false);
        GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
        if (targetMove == Vector3.zero && currentAction == ActionUnite.mouv)
        {
            if (Vector3.Distance(transform.position, targetFinishMove) < 1)
                currentAction = ActionUnite.idle;
            else
                ChangeAction(ActionUnite.mouvAttak, targetFinishMove);
        }

        if (targetMove != Vector3.zero)
        {
            Move();
        }
        if (target != null)
        {
            LaunchAttaque();
            if ((target.transform.position - transform.position).magnitude <= range)
                StopToHitDistance();
            if (targetMove == Vector3.zero && (target.transform.position - transform.position).magnitude > range)
                Aggro();
        }
        if (isRecolteur)
            RessourceManagement();

        if (isCorpseRecolteur)
        {
            target = FindTheCloseEnemy(true);
            if (target != null && (target.transform.position - transform.position).magnitude <= 2)
                PickUpCorpse();
        }
    }

    public void ModifMoralGlobal()
    {
        if (currentMoral > middleMoral)
            MoralChanger(-1);
        else if (currentMoral < middleMoral)
            MoralChanger(1);
    }

    private void RessourceManagement()
    {
        if (isCharged && (village.transform.position - transform.position).magnitude < 3)
        {
            isCharged = false;
            if (!isUndead)
                village.GetComponent<Village>().AddRessource();
            TargetMove(ressZone.transform.position);
        }
        if (!isCharged && (ressZone.transform.position - transform.position).magnitude < 3)
        {
            isCharged = true;
            TargetMove(village.transform.position);
        }
    }

    [Command(requiresAuthority = false)]
    public void TriggerSkill(int idSkill, Vector3 target)
    {
        Debug.Log(skillsList.Count);
        skillsList[idSkill].Trigger(gameObject, VectorZeroY(target));
    }
    public void PickUpCorpse()
    {
        flesh += target.GetComponent<Bones>().flesh;
        bone += target.GetComponent<Bones>().bone;
        target.GetComponent<Bones>().PickUped();
    }
    public void StopToHitDistance()
    {
        targetMove = Vector3.zero;
    }
    private void Aggro()
    {
        agent.destination = target.transform.position;
        RotationToUnit();
    }

    private void RotationToUnit()
    {
        Vector3 lookTo = VectorZeroY(targetFinishMove - transform.position);
        GetComponentInChildren<Rigidbody>().transform.rotation = Quaternion.FromToRotation(Vector3.forward, lookTo);
    }
    public void Selected(bool isSelect)
    {
        if (isSelect)
            GetComponentInChildren<MeshRenderer>().materials = selectedMaterial;
        else
            GetComponentInChildren<MeshRenderer>().materials = normalMaterial;
    }
    public void InitHeal()
    {
        if (isServer)
            currentHealth = maxHealth;
    }
    public void LifeSteal(int damage, TypeOfDamage type, Unit caster)
    {
        if (isServer)
        {
            switch (type)
            {
                case TypeOfDamage.contandant:
                    damage -= defCont;
                    break;
                case TypeOfDamage.tranchant:
                    damage -= defTran;
                    break;
                case TypeOfDamage.magique:
                    damage -= defMagi;
                    break;
                default:
                    break;
            }
            damage = Mathf.Max(0, damage);
            currentHealth -= damage;
            caster.Heal(damage);
        }
        else
            CmdTakeDamage(damage);
    }
    public void TakeDamage(int damage, TypeOfDamage type)
    {
        if (isServer)
        {
            switch (type)
            {
                case TypeOfDamage.contandant:
                    damage -= defCont;
                    break;
                case TypeOfDamage.tranchant:
                    damage -= defTran;
                    break;
                case TypeOfDamage.magique:
                    damage -= defMagi;
                    break;
                default:
                    break;
            }
            damage = Mathf.Max(0, damage);
            currentHealth -= damage;
        }
        else
            CmdTakeDamage(damage);
    }
    public void Heal(int heal)
    {
        if (isServer)
            currentHealth = Mathf.Min(currentHealth + heal, maxHealth);
        else
            CmdHeal(heal);
    }
    [Command]
    public void CmdHeal(int heal)
    {
        Heal(heal);
    }
    [Command]
    public void CmdTakeDamage(int damage)
    {
        TakeDamage(damage, typeOfDamage);
    }
    void HealthChanged(int oldValue, int newValue)
    {
        if (newValue <= 0 && isAlive)
        {
            isAlive = false;
            if (!isServer)
                return;
            if (isUndead)
                GameManager.instance.manaReserve -= manaReserve;
            else
                GameManager.instance.SetConsumtion(-consomptionFood);

            if (UnityEngine.Random.Range(0, 100) < probaDropCorpse)
            {
                GameObject go = Instantiate(corpse, transform.position, Quaternion.identity);
                go.GetComponent<Bones>().flesh = corpseFlesh;
                go.GetComponent<Bones>().bone = corpseBone;
                GameManager.instance.SpawnGameObject(go);
            }
            GameManager.instance.ServerDestroy(gameObject);
        }
        else if (healthBarre != null)
            healthBarre.fillAmount = (float)currentHealth / (float)maxHealth;
    }


    public void LaunchAttaque()
    {
        if (Time.time > tickAttaque && (target.transform.position - transform.position).magnitude < range)
        {
            TargetMove(Vector3.zero);
            tickAttaque = Time.time + cdAttaque;
            int currentDamage = damage;
            if (CompareTag("Humans"))
            {
                if (currentEtat == StatMoral.determined)
                {
                    int alea = UnityEngine.Random.Range(0, 100);
                    if (alea < 20)
                        currentDamage = damage * 2;
                }
                else if (currentEtat == StatMoral.scared)
                {
                    int alea = UnityEngine.Random.Range(0, 100);
                    if (alea < 20)
                        currentDamage = damage / 2;
                }
            }
            else if (CompareTag("Undead"))
            {
                if (target.GetComponentInParent<Unit>().currentEtat == StatMoral.scared)
                {
                    int alea = UnityEngine.Random.Range(0, 100);
                    if (alea < 20)
                        currentDamage = damage * 2;
                }
            }
            if (target.GetComponentInParent<Unit>() != null)
            {
                if (projectile != null)
                    CmdSpawnProjectile();
                target.GetComponentInParent<Unit>().TakeDamage(currentDamage, typeOfDamage);
                if (isHero)
                {
                    int dmgEffectif = currentDamage;
                    if (typeOfDamage == TypeOfDamage.tranchant)
                        dmgEffectif -= target.GetComponentInParent<Unit>().defTran;
                    else if (typeOfDamage == TypeOfDamage.contandant)
                        dmgEffectif -= target.GetComponentInParent<Unit>().defCont;
                    else if (typeOfDamage == TypeOfDamage.magique)
                        dmgEffectif -= target.GetComponentInParent<Unit>().defMagi;
                    if (dmgEffectif > 0)
                        GameManager.instance.GestionDgtHeroForXp(dmgEffectif);
                }
            }
            else
                Debug.LogError("Une attaque ne trouve pas de script adequate pour etre infligé", gameObject);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdSpawnProjectile()
    {
        RpcSpawnGameObject();
    }
    [ClientRpc]
    public void RpcSpawnGameObject()
    {
        if (!isServer)
            return;
        GameObject go = Instantiate(projectile, transform.position + 2 * Vector3.up, Quaternion.identity);
        go.GetComponent<Projectiles>().target = target;
        NetworkServer.Spawn(go);
    }
    public void Move()
    {
        if ((targetMove - transform.position).magnitude > 1)
        {
            isMoving = true;
            Vector3 deplacement = (targetMove - transform.position).normalized;
            transform.Translate(VectorZeroY(deplacement) * speed * Time.fixedDeltaTime);
        }
        else
        {
            isMoving = false;
            currentAction = ActionUnite.idle;
            targetMove = Vector3.zero;

            //pour les charrettes
            if (isTransport)
            {
                if (isReturn) // arrive au chateau
                {
                    GameManager.instance.SetWood(wood);
                    GameManager.instance.SetIron(iron);
                    GameManager.instance.SetFood(food);
                    iron = food = wood = 0;
                }
                else // arrive au village
                {
                    Village v = targetVillage.GetComponent<Village>();
                    switch (v.typeOfProd)
                    {
                        case Ressources.food:
                            food = Mathf.Min(stockMax, v.food);
                            v.food -= food;
                            break;
                        case Ressources.wood:
                            wood = Mathf.Min(stockMax, v.wood);
                            v.wood -= wood;
                            break;
                        case Ressources.iron:
                        default:
                            iron = Mathf.Min(stockMax, v.iron);
                            v.iron -= iron;
                            break;
                    }
                }
                GoStartVillage();
            }
        }
    }

    public GameObject FindTheCloseEnemy(bool isCorpseSearch)
    {
        List<GameObject> targets;
        if (isCorpseSearch)
            targets = GameObject.FindGameObjectsWithTag("Bones").Where(e => (e.transform.position - transform.position).magnitude < rangeToAggro).ToList();
        else
            targets = isUndead ? GameObject.FindGameObjectsWithTag("Humans").Where(e => (e.transform.position - transform.position).magnitude < rangeToAggro).ToList() :
                                                  GameObject.FindGameObjectsWithTag("Undead").Where(e => (e.transform.position - transform.position).magnitude < rangeToAggro).ToList();

        if (targets.Count != 0)
        {
            return targets.OrderBy(e => (e.transform.position - transform.position).magnitude).First();
        }
        return null;
    }

    public void AddSkill(int idSkill)
    {
        Skills s = new Skills();
        s.UseBDD(idSkill);
        skillsList.Add(s);
    }
    [Command(requiresAuthority = false)]
    public void AddNewSkill(int idSkill)
    {
        Skills s = new Skills();
        s.UseBDD(idSkill);
        skillsList.Add(s);
        //RpcAddSkill(s);
    }
    [ClientRpc]
    public void RpcAddSkill(Skills s)
    {
        skillsList.Add(s);
    }
    public void UseBDD()
    {
        UniteBDD bdd = Resources.Load("UniteBDD") as UniteBDD;
        UniteData ud = bdd.Get(id);

        if (ud.IsUndead)
        {
            tag = "Undead";
            manaReserve = ud.ManaReserve;
        }
        else
        {
            tag = "Humans";
            consomptionFood = ud.FoodComsomption;
            currentEtat = StatMoral.normal;
            middleMoral = ud.MiddleMoral;
            currentMoral = middleMoral;
            maxMoral = ud.MaxMoral;
            moralSeuilFuite = ud.SeuilFuiteMoral;
            moralSeuilBas = ud.SeuilBasMoral;
            moralSeuilHaut = ud.SeuilHautMoral;
        }
        maxHealth = ud.MaxHealth;
        damage = ud.Damage;
        typeOfDamage = ud.TypeDamage;
        defCont = ud.DefContandant;
        defTran = ud.DefTranchant;
        defMagi = ud.DefMagique;
        speed = ud.Speed;
        range = ud.Range;
        fieldOfView = ud.FieldOfView;
        transform.Find("FoV").transform.localScale = new Vector3(fieldOfView, 0, fieldOfView);
        rangeToAggro = fieldOfView - 2;
        cdAttaque = ud.CdAttack;
        if (ud.SkillsList.Count > 0)
        {
            for (int i = 0; i < ud.SkillsList.Count; i++)
            {
                AddSkill(ud.SkillsList[i].Id);
                //Skills s = new Skills();
                //s.UseBDD(ud.SkillsList[i].Id);
                //skillsList.Add(s);
            }
        }
        if (ud.Projectile != null)
            projectile = ud.Projectile;

        if (ud.ProbaCorpse > 0)
        {
            probaDropCorpse = ud.ProbaCorpse;
            idCorpse = ud.IdCorpse;
            corpseFlesh = ud.Flesh;
            corpseBone = ud.Bone;
        }

        if (ud.IsTransport)
        {
            isTransport = ud.IsTransport;
            stockMax = ud.MaxStock;
        }

        if (ud.IsRecolteur)
        {
            isRecolteur = ud.IsRecolteur;
        }
        if (ud.IsCorpseRecolteur)
        {
            isCorpseRecolteur = ud.IsCorpseRecolteur;
        }
    }
    public Vector3 VectorZeroY(Vector3 entree)
    {
        return new Vector3(entree.x, 0, entree.z);
    }
}
