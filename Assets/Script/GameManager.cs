using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    [Header("Map")]
    public static GameManager instance;
    public UniteBDD unitBDD;
    public SkillBDD skillBDD;
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private GameObject[] batiments;

    [Header("Necro")]
    private float tickCalculRessource;
    public float timeCalculRessource;
    public PlayerController playerNecro;
    [SyncVar] public List<GameObject> armeeNecro;
    [SyncVar(hook = nameof(ChangeUINecro))]
    public float mana = 50;
    [SyncVar(hook = nameof(ChangeUINecro))]
    public float manaMax = 100;
    [SyncVar(hook = nameof(ChangeUINecro))]
    public float manaReserve = 0;
    [SyncVar(hook = nameof(ChangeUINecro))]
    public float manaRegen = 1f;
    public float manaRegenUp;
    [HideInInspector] public int UpPvUnit, UpPvUnitPurcent, UpDamageUnit, UpDamageUnitPurcent;
    [HideInInspector] public float UpSpeedUnit;

    [Header("Human")]
    [SyncVar] public List<GameObject> armeeHuman;
    [HideInInspector] public List<PlayerController> playersHumans;
    private int dmgForXpHero;
    [SyncVar(hook = nameof(ChangeUIHumans))]
    public float food;
    [SyncVar(hook = nameof(ChangeUIHumans))]
    public float consumptionFood;
    [SyncVar(hook = nameof(ChangeUIHumans))]
    public float wood;
    [SyncVar(hook = nameof(ChangeUIHumans))]
    public float iron;

    [SyncVar] public GameObject castle;
    public SyncList<GameObject> villages = new SyncList<GameObject>();
    private void Awake()
    {
        if (instance != null)
            return;
        instance = this;
        playersHumans = new List<PlayerController>();
        tickCalculRessource = Time.time + timeCalculRessource;
        foreach (var item in GameObject.FindGameObjectsWithTag("Village"))
        {
            villages.Add(item);
        }
        armeeNecro.Add(GameObject.Find("Necro"));
        armeeNecro.Add(GameObject.Find("Chariot"));
        armeeHuman.Add(castle);
    }
    private void Update()
    {
        if (!isServer)
            return;

        if (armeeHuman.Count > 0)
            armeeHuman = armeeHuman.Where(item => item != null).ToList();
        if (armeeNecro.Count > 0)
            armeeNecro = armeeNecro.Where(item => item != null).ToList();

        if (Time.time > tickCalculRessource && playersHumans.Count > 0)
        {
            /// regeneration de mana du nécromancien
            tickCalculRessource = Time.time + timeCalculRessource;
            mana = Mathf.Min(mana + manaRegen, manaMax - manaReserve);

            /// gestion humaine
            ApproHumans();
            foreach (Unit u in armeeHuman.Select(e => e.GetComponent<Unit>()))
            {
                u.ModifMoralGlobal();
            }
            Debug.Log("nb de villages " + villages.Select(e => e.GetComponent<Village>()).Count());
            foreach (Village v in villages.Select(e => e.GetComponent<Village>()))
            {
                v.ModifVillageMoralGlobal();
            }
        }
    }
    #region ressources
    [Server]
    public void ApproHumans()
    {
        SetFood(-consumptionFood);
        while (food < 0)
        {
            food++;
            List<Unit> unitConsomante = armeeHuman.Where(e => e.GetComponent<Unit>().consomptionFood != 0).Select(e => e.GetComponent<Unit>()).ToList();
            int rdm = Random.Range(0, unitConsomante.Count);
            unitConsomante[rdm].TakeDamage(1000, TypeOfDamage.brut);
        }
    }
    public void GestionDgtHeroForXp(int dmg)
    {
        dmgForXpHero += dmg;
        if (dmgForXpHero >= 10)
        {
            int i = dmgForXpHero / 10;
            RpcGainXp(XpOnVoie.Mage, i, 0);
            dmgForXpHero -= 10 * i;
        }
    }
    [ClientRpc]
    public void RpcGainXp(XpOnVoie xpOnVoie, float baseXpHumans, float baseXpNecro)
    {
        foreach (PlayerController p in playersHumans)
        {
            p.GestionXpUp(xpOnVoie, baseXpHumans);
        }
        if (baseXpNecro > 0)
            playerNecro.GestionXpUpNecro(baseXpNecro);
    }
    public void ChangeUINecro(float old, float n)
    {
        if (playerNecro != null)
            playerNecro.ChangeUIRessources();
    }
    public void ChangeUIHumans(float old, float n)
    {
        if (playersHumans.Count() > 0 && playersHumans.Any(e => e != null))
        {
            foreach (PlayerController p in playersHumans)
            {
                p.ChangeUIRessources();
                if (n > old)
                    p.GestionXpUp(XpOnVoie.Magistrat, n - old);
            }
        }
    }
    public void UpManaRegenUp(float bonus)
    {
        manaRegenUp += bonus;
        CountRegenerationMana();
    }
    public void CountRegenerationMana()
    {
        if (isServer)
        {
            float newRegen = 1;
            foreach (GameObject village in GameObject.FindGameObjectsWithTag("Village").Where(v => v.GetComponent<Village>() != null))
            {
                newRegen += village.GetComponent<Village>().popAcolyte.Count * 0.1f;
            }
            manaRegen = newRegen + manaRegenUp;
        }
        else
            CmdCountRegenerationMana();
    }
    public void SetManaMax(float i)
    {
        if (isServer)
            manaMax += i;
        else
            CmdSetManaMax(i);
    }
    [Command]
    public void CmdSetManaMax(float i)
    {
        SetManaMax(i);
    }
    [Command]
    public void CmdCountRegenerationMana()
    {
        CountRegenerationMana();
    }
    public void SetFood(float i)
    {
        if (isServer)
            food += i;
        else
            CmdSetFood(i);
    }
    [Command]
    public void CmdSetFood(float i)
    {
        SetFood(i);
    }
    public float GetFood()
    {
        return food;
    }
    public void SetConsumtion(float i)
    {
        if (isServer)
            consumptionFood += i;
        else
            CmdSetConsumtion(i);
    }
    [Command]
    public void CmdSetConsumtion(float i)
    {
        SetConsumtion(i);
    }
    public float GetConsumtion()
    {
        return consumptionFood;
    }
    public void SetWood(float i)
    {
        if (isServer)
            wood += i;
        else
            CmdSetWood(i);
    }
    [Command]
    public void CmdSetWood(float i)
    {
        SetWood(i);
    }
    public float GetWood()
    {
        return wood;
    }
    public void SetIron(float i)
    {
        if (isServer)
            iron += i;
        else
            CmdSetIron(i);
    }
    [Command]
    public void CmdSetIron(float i)
    {
        SetIron(i);
    }
    public float GetIron()
    {
        return iron;
    }
    public void SetMana(float i)
    {
        if (isServer)
            mana += i;
        else
            CmdSetMana(i);
    }
    [Command]
    public void CmdSetMana(float i)
    {
        SetMana(i);
    }
    public float GetMana()
    {
        return mana;
    }
    #endregion
    public void CreateMap()
    {
        Debug.Log("Mise en place des paysans de base");
        foreach (GameObject village in GameObject.FindGameObjectsWithTag("Village").Where(v => v.GetComponent<Village>() != null))
        {
            int popInitiale = Random.Range(3, 5);
            for (int i = 0; i < popInitiale; i++)
            {
                village.GetComponent<Village>().CreateOnePaysan();
            }
        }
    }
    /// <summary>
    /// Detruire un objet sur le serveur (et gerer l'xp si necessaire)
    /// </summary>
    /// <param name="go">objet</param>
    /// <param name="timeToDeath">temps avant la destruction</param>
    public void ServerDestroy(GameObject go, float timeToDeath = 0)
    {
        if (isServer)
        {
            if (go.GetComponent<Unit>() != null)
            {
                if (go.GetComponent<Unit>().isUndead)
                    RpcGainXp(XpOnVoie.Martial, 3, 1);
                else
                    RpcGainXp(XpOnVoie.Martial, 1, 3);
            }
            if (timeToDeath != 0)
                StartCoroutine(DoServerDestroy(go, timeToDeath));
            else
                NetworkServer.Destroy(go);
        }
        else
            CmdServerDestroy(go);
    }

    [Server]
    internal IEnumerator DoServerDestroy(GameObject go, float timeToDeath)
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(go);
    }
    [Command]
    public void CmdServerDestroy(GameObject go)
    {
        ServerDestroy(go);
    }

    [ClientRpc]
    public void RpcCostHUUnit(int costIron, int costWood, int costFood, float consumption)
    {
        if (!isServer)
            return;
        iron -= costIron;
        wood -= costWood;
        food -= costFood;
        consumptionFood += consumption;
    }
    [ClientRpc]
    public void RpcCostUDUnit(Unit chariot, int costBone, int costFlesh, float reserve)
    {
        if (!isServer)
            return;
        chariot.flesh -= costFlesh;
        chariot.bone -= costBone;
        manaReserve += reserve;
    }
    [Command(requiresAuthority = false)]
    public void CmdCostAndCreateUnitSkill(int idSkillData, int idUnitData, Unit caster, Vector3 position)
    {
        CostAndCreateUnitSkill(idSkillData, idUnitData, caster, position);
    }

    public void CostAndCreateUnitSkill(int idSkillData, int idUnitData, Unit caster, Vector3 position)
    {
        if (isServer)
        {
            SkillData sd = skillBDD.Get(idSkillData);
            UniteData ud = unitBDD.Get(idUnitData);
            if (sd == null)
                Debug.LogError("Le skill n'est pas datacié");
            if (ud == null)
                Debug.LogError("L'unite n'est pas datacié");

            if ((ud.IsUndead && !sd.IsUndead) || (!ud.IsUndead && sd.IsUndead))
            {
                Debug.LogError("Le sort et l'unité créé ne sont pas de la même equipe");
                return;
            }

            if (ud.IsUndead)
            {
                if (caster.flesh >= sd.CostFlesh && caster.bone >= sd.CostBone)
                {
                    RpcCreateUnit(idUnitData, position);
                    RpcCostUDUnit(caster, sd.CostBone, sd.CostFlesh, ud.ManaReserve);
                }
            }
            else
            {
                if (food >= sd.CostFood && wood >= sd.CostWood && iron >= sd.CostIron)
                {
                    RpcCreateUnit(idUnitData, position);
                    RpcCostHUUnit(sd.CostIron, sd.CostWood, sd.CostFood, ud.FoodComsomption);
                    RpcGainXp(XpOnVoie.Magistrat, 5, 0);
                }
            }
        }
        else
            CmdCostAndCreateUnitSkill(idSkillData, idUnitData, caster, position);
    }
    [Command(requiresAuthority = false)]
    public void CmdCostAndCreateCharetteSkill(int idSkillData, int idUnitData, Unit caster, Vector3 position, GameObject villageTarget)
    {
        CostAndCreateCharette(idSkillData, idUnitData, caster, position, villageTarget);
    }
    public void CostAndCreateCharette(int idSkillData, int idUnitData, Unit caster, Vector3 position, GameObject villageTarget)
    {
        if (isServer)
        {
            SkillData sd = skillBDD.Get(idSkillData);
            UniteData ud = unitBDD.Get(idUnitData);
            if (sd == null)
                Debug.LogError("Le skill n'est pas datacié");
            if (ud == null)
                Debug.LogError("L'unite n'est pas datacié");

            if ((ud.IsUndead && !sd.IsUndead) || (!ud.IsUndead && sd.IsUndead))
            {
                Debug.LogError("Le sort et l'unité créé ne sont pas de la même equipe");
                return;
            }
            if (food >= sd.CostFood && wood >= sd.CostWood && iron >= sd.CostIron)
            {
                RpcCreateCharette(idUnitData, position, villageTarget);
                RpcCostHUUnit(sd.CostIron, sd.CostWood, sd.CostFood, ud.FoodComsomption);

            }
        }
        else
            CmdCostAndCreateCharetteSkill(idSkillData, idUnitData, caster, position, villageTarget);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (FindObjectsOfType<GameObject>().Any(e => e.GetComponent<PlayerController>() != null && !e.GetComponent<PlayerController>().isAssign))
        {
            PlayerController player = FindObjectsOfType<GameObject>().FirstOrDefault(e => e.GetComponent<PlayerController>() != null && !e.GetComponent<PlayerController>().isAssign).GetComponent<PlayerController>();

            if (FindObjectsOfType<GameObject>().FirstOrDefault(e => e.name.Contains("player")) == null)
            {
                string id = "player " + GetComponent<NetworkIdentity>().netId;

                player.transform.name = id;
                player.lvlToChoice = 3;
                player.gameObject.tag = "Undead";
                player.uiPlayer.SetUiForNecro();
                player.isAssign = true;
                CreateMap();
                playerNecro = player;
                RpcAssignUndead(player);
            }
        }
        else
        {
            CmdAssignHumans();
        }
    }
    [ClientRpc]
    public void RpcAssignUndead(PlayerController player)
    {
        player.transform.position = new Vector3(230, 18, 135);
    }
    [Command(requiresAuthority = false)]
    public void CmdAssignHumans()
    {
        PlayerController player = FindObjectsOfType<GameObject>().FirstOrDefault(e => e.GetComponent<PlayerController>() != null && !e.GetComponent<PlayerController>().isAssign).GetComponent<PlayerController>();
        string id = "player " + GetComponent<NetworkIdentity>().netId;

        player.transform.name = id;
        player.isAssign = true;

        if (armeeHuman.Count == 0)
            armeeHuman.Add(GameObject.Find("Castle"));
        CreateHero(13, new Vector3(-120, 0, -25), player);
        playersHumans.Add(player);
        player.xpPlayer = player.transform.GetComponentInChildren<XpHumansScript>();
        player.lvlToChoice = 1;
        RpcAssignHumans(player);
    }
    [ClientRpc]
    public void RpcAssignHumans(PlayerController player)
    {
        playersHumans.Add(player);
        if (playersHumans.Any(e => e == player))
            player.gameObject.tag = "Humans";
        player.uiPlayer.SetUiForHuman();
        player.transform.position = new Vector3(-120, 15, -45);

    }
    public void CreateHero(int idUdd, Vector3 zoneCreate, PlayerController p)
    {
        UniteData udd = unitBDD.Get(idUdd);

        consumptionFood += 1;

        GameObject go = Instantiate(udd.Prefab, zoneCreate + Vector3.back * 3, Quaternion.identity);
        if (go == null)
            Debug.LogError("Unite non instancié");
        armeeHuman.Add(go);
        NetworkServer.Spawn(go);
        RpcAssignXPHero(p, go.GetComponent<Unit>());
    }
    [ClientRpc]
    public void RpcAssignXPHero(PlayerController p, Unit unit)
    {
        p.AssignXpPanelToHero(unit);
    }

    [ClientRpc]
    public void RpcCreateUnit(int idUdd, Vector3 zoneCreate)
    {
        if (!isServer)
            return;
        UniteData udd = unitBDD.Get(idUdd);

        GameObject go = Instantiate(udd.Prefab, zoneCreate + Vector3.back * 3, Quaternion.identity);
        if (go == null)
            Debug.LogError("Unite non instancié");
        if (udd.IsUndead)
        {
            UpUnitNecro(go.GetComponent<Unit>());
            armeeNecro.Add(go);
        }
        else
        {
            armeeHuman.Add(go);
        }
        NetworkServer.Spawn(go);
    }
    [ClientRpc]
    public void RpcCreateCharette(int idUdd, Vector3 zoneCreate, GameObject villageTarget)
    {
        if (!isServer)
            return;
        UniteData udd = unitBDD.Get(idUdd);

        GameObject go = Instantiate(udd.Prefab, zoneCreate, Quaternion.identity);
        if (go == null)
            Debug.LogError("Unite non instancié");
        go.GetComponent<Unit>().targetVillage = villageTarget;
        armeeHuman.Add(go);

        NetworkServer.Spawn(go);
    }

    public void UpUnitNecro(Unit newUnit)
    {
        newUnit.maxHealth = (int)(newUnit.maxHealth * (100f + UpPvUnitPurcent) / 100f);
        newUnit.maxHealth += UpPvUnit;

        newUnit.damage = (int)(newUnit.maxHealth * (100f + UpDamageUnitPurcent) / 100f);
        newUnit.damage += UpDamageUnit;
        newUnit.speed += UpSpeedUnit;
        if (newUnit.speed != 0)
            newUnit.agent.speed = newUnit.speed;
    }
    public void AddUpPvUnitPurcent(int bonus)
    {
        UpPvUnitPurcent += bonus;
    }
    public void AddUpPvUnit(int bonus)
    {
        UpPvUnit += bonus;
    }
    public void AddUpDamageUnitPurcent(int bonus)
    {
        UpDamageUnitPurcent += bonus;
    }
    public void AddUpDamageUnit(int bonus)
    {
        UpDamageUnit += bonus;
    }
    public void AddUpSpeedUnit(float bonus)
    {
        UpSpeedUnit += bonus;
    }
    public void SpawnGameObject(GameObject go)
    {
        if (go.CompareTag("Undead"))
        {
            armeeNecro.Add(go);
            manaReserve += 1;
        }
        else if (go.CompareTag("Humans"))
        {
            armeeHuman.Add(go);
        }
        NetworkServer.Spawn(go);
    }

    [ClientRpc]
    public void RpcSpawnCharette(int[] orders, GameObject villageTarget)
    {
        if (!isServer)
            return;
        UniteData udd = unitBDD.Get(4);
        if (udd == null)
        {
            Debug.LogError("ne trouve pas la Data Unit");
        }
        GameObject castle = GameObject.Find("Castle");
        GameObject charrette = Instantiate(udd.Prefab, castle.transform.position, Quaternion.identity);
        armeeHuman.Add(charrette);
        NetworkServer.Spawn(charrette);
        Unit u = charrette.GetComponent<Unit>();
        if (u == null)
        {
            Debug.LogError("ne trouve pas le script Unit");
        }
        u.targetVillage = villageTarget;
        u.food = Mathf.Max(0, -orders[0]);
        u.wood = Mathf.Max(0, -orders[1]);
        u.iron = Mathf.Max(0, -orders[2]);
        u.colon = Mathf.Max(0, -orders[3]);
        u.orders = orders;
    }

    public void ValidateCharette(int[] orders, GameObject villageTarget)
    {
        if (isServer)
        {
            RpcSpawnCharette(orders, villageTarget);

        }
        else
            CmdValidateCharette(orders, villageTarget);
    }
    [Command]
    public void CmdValidateCharette(int[] orders, GameObject villageTarget)
    {
        ValidateCharette(orders, villageTarget);
    }

    [ClientRpc]
    public void RpcSpawnPaysan(GameObject village)
    {
        if (!isServer)
            return;
        UniteData udd = unitBDD.Get(0);
        GameObject paysan = Instantiate(udd.Prefab, village.transform.position, Quaternion.identity);
        NetworkServer.Spawn(paysan);
        Unit p = paysan.GetComponent<Unit>();
        p.village = village;
        p.ressZone = village.GetComponent<Village>().ChoiceOneChamp();
        p.TargetMove(p.ressZone.transform.position);
        village.GetComponent<Village>().AddPaysan(paysan);
    }

    [Server]
    public void SpawnPaysan(GameObject village)
    {
        if (isServer)
            RpcSpawnPaysan(village);
    }

}
