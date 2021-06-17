using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class Village : NetworkBehaviour
{
    private float timeProduction;
    private float tickProduction;
    public Ressources typeOfProd;
    public GameObject[] ressourceChamps;
    [SyncVar] public int wood, food, iron;
    [SyncVar] public int stockMax;
    [SyncVar] public List<GameObject> popPaysan;
    [SyncVar] public List<GameObject> popAcolyte;
    [SyncVar] public int popMax;
    public Material selectedMaterial, normalMaterial;
    public Material udSelectedMaterial, udNormalMaterial;
    // Start is called before the first frame update
    void Start()
    {
        timeProduction = 15;
        tickProduction = Time.time + timeProduction;
        popMax = 20;
        stockMax = 40;
        for (int i = 0; i < ressourceChamps.Length; i++)
        {
            if(Vector3.Distance(transform.position, ressourceChamps[i].transform.position) < 8 || Vector3.Distance(transform.position, ressourceChamps[i].transform.position) > 13)
            Debug.Log($"{name} : {Vector3.Distance(transform.position, ressourceChamps[i].transform.position)}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && tickProduction != 0 && Time.time >= tickProduction)
        {
            CleanList();
            if (popPaysan.Count == 0 || popPaysan.Count <= popAcolyte.Count)
            {
                return;
            }
            else
            {
                tickProduction = Time.time + timeProduction;
                if (popPaysan.Count < popMax)
                {
                    CreateOnePaysan();
                }
            }
        }
    }

    public void AddPaysan(GameObject paysan)
    {
        popPaysan.Add(paysan);
    }
    [Server]
    public void CreateOnePaysan()
    {
        GameManager.instance.SpawnPaysan(gameObject);
    }
    public GameObject ChoiceOneChamp()
    {
        return ressourceChamps[Random.Range(0, ressourceChamps.Length)];
    }

    public List<string> Stats(bool isUndead)
    {
        List<string> result = new List<string>();
        result.Add(gameObject.name);
        result.Add($"Produit {typeOfProd}");
        result.Add($"{popPaysan.Count} / {popMax} Villageois");
        result.Add($"{food}food - {wood}wood - {iron}iron");
        result.Add($"{food + wood + iron} / {stockMax} Stock");
        if (isUndead)
            result.Add($"{popAcolyte.Count} Acolytes ");
        return result;
    }
    public void Selected(bool isSelect)
    {
        if (isSelect)
            GetComponentInChildren<MeshRenderer>().material = selectedMaterial;
        else
            GetComponentInChildren<MeshRenderer>().material = normalMaterial;
    }
    /// <summary>
    /// fais remonter le moral de toutes les unités militaires autour du village
    /// </summary>
    public void ModifVillageMoralGlobal()
    {        
        foreach (Unit u in GameManager.instance.armeeHuman.Where(e => Vector3.Distance(e.transform.position, transform.position) < 18 && e.GetComponent<Unit>() != null).Select(e => e.GetComponent<Unit>()))
        {
            u.MoralChanger(5);
        }
    }
    public void CorruptAcolyte()
    {
        if (isServer)
        {
            CleanList();
            if (popPaysan.Any(p => !p.GetComponent<Unit>().isUndead))
            {
                GameObject[] noAcolytes = popPaysan.Where(p => !p.GetComponent<Unit>().isUndead).ToArray();
                int random = Random.Range(0, noAcolytes.Length);
                noAcolytes[random].GetComponent<Unit>().isUndead = true;
                noAcolytes[random].tag = "Undead";
                GameManager.instance.armeeNecro.Add(noAcolytes[random]);
                popAcolyte.Add(noAcolytes[random]);
                ChangeToUndead();
            }
        }
        else
            CmdCorruptAcolyte();
    }
    [Command]
    public void CmdCorruptAcolyte()
    {
        CorruptAcolyte();
    }
    public void SacrificeAcolyteToKill()
    {
        if (isServer)
        {
            CleanList();
            popAcolyte[0].GetComponent<Unit>().TakeDamage(popAcolyte[0].GetComponent<Unit>().maxHealth, TypeOfDamage.brut);
            Unit victime = popPaysan.First(p => !p.GetComponent<Unit>().isUndead).GetComponent<Unit>();
            victime.TakeDamage(victime.maxHealth, TypeOfDamage.brut);
            CleanList();
            ChangeToUndead();
        }
        else
            CmdSacrificeAcolyteToKill();
    }

    public void AddRessource()
    {
        int stock = food + wood + iron;
        if (stockMax > stock)
        {
            switch (typeOfProd)
            {
                case Ressources.food:
                    food++;
                    break;
                case Ressources.iron:
                    iron++;
                    break;
                case Ressources.wood:
                    wood++;
                    break;
            }
        }
    }

    [Command]
    public void CmdSacrificeAcolyteToKill()
    {
        SacrificeAcolyteToKill();
    }
    [Server]
    public void ChangeToUndead()
    {
        if (tickProduction != 0 && !popPaysan.Any(p => !p.GetComponent<Unit>().isUndead))
        {
            tickProduction = 0;
            GameManager.instance.SetManaMax(10);
            selectedMaterial = udSelectedMaterial;
            normalMaterial = udNormalMaterial;
            GetComponentInChildren<MeshRenderer>().material = normalMaterial;
            RpcChangeToUndead();
        }
    }
    [ClientRpc]
    public void RpcChangeToUndead()
    {
        selectedMaterial = udSelectedMaterial;
        normalMaterial = udNormalMaterial;
        GetComponentInChildren<MeshRenderer>().material = normalMaterial;
    }

    [Server]
    public void CleanList()
    {
        if (popPaysan != null)
        {
            popPaysan.RemoveAll(GameObject => GameObject == null);
            popPaysan.RemoveAll(p => !p.GetComponent<Unit>().isAlive);
        }

        if (popAcolyte != null)
        {
            popAcolyte.RemoveAll(GameObject => GameObject == null);
            popAcolyte.RemoveAll(p => !p.GetComponent<Unit>().isAlive);
        }
        ChangeToUndead();
        RpcCleanList();
    }
    [ClientRpc]
    public void RpcCleanList()
    {
        if (popPaysan != null)
            popPaysan.RemoveAll(GameObject => GameObject == null);

        if (popAcolyte != null)
            popAcolyte.RemoveAll(GameObject => GameObject == null);
    }
}
