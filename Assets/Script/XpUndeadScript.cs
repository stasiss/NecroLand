using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class XpUndeadScript : MonoBehaviour 
{
    public PlayerController p;
    public GameObject[] voiceGeneralSkills;
    public Text descriptionText;
    public ButtonWithDescription[] buttonsVoie1;
    [HideInInspector] public bool[] isSelectVoie1;
    public ButtonWithDescription[] buttonsVoie2;
    [HideInInspector] public bool[] isSelectVoie2;
    public ButtonWithDescription[] buttonsVoie3;
    [HideInInspector] public bool[] isSelectVoie3;
    public UniteBDD unitBDD;
    public LvlData[] voie1;
    public LvlData[] voie2;
    public LvlData[] voie3;
    // Start is called before the first frame update
    void Start()
    {
        isSelectVoie1 = new bool[buttonsVoie1.Length];
        isSelectVoie1[0] = true;
        buttonsVoie1[0].GetComponent<Image>().color = Color.white;
        isSelectVoie1[4] = true;
        buttonsVoie1[4].GetComponent<Image>().color = Color.white;
        isSelectVoie1[8] = true;
        buttonsVoie1[8].GetComponent<Image>().color = Color.white;

        isSelectVoie2 = new bool[buttonsVoie2.Length];
        isSelectVoie2[0] = true;
        buttonsVoie2[0].GetComponent<Image>().color = Color.white;
        isSelectVoie2[4] = true;
        buttonsVoie2[4].GetComponent<Image>().color = Color.white;
        isSelectVoie2[8] = true;
        buttonsVoie2[8].GetComponent<Image>().color = Color.white;

        isSelectVoie3 = new bool[buttonsVoie3.Length];
        isSelectVoie3[0] = true;
        buttonsVoie3[0].GetComponent<Image>().color = Color.white;
        isSelectVoie3[4] = true;
        buttonsVoie3[4].GetComponent<Image>().color = Color.white;
        isSelectVoie3[8] = true;
        buttonsVoie3[8].GetComponent<Image>().color = Color.white;

        int maxBoucle = Mathf.Min(buttonsVoie1.Length, voie1.Length);
        for (int i = 0; i < maxBoucle; i++)
        {
            buttonsVoie1[i].GetComponentInChildren<Text>().text = voie1[i].Name;
            buttonsVoie2[i].GetComponentInChildren<Text>().text = voie2[i].Name;
            buttonsVoie3[i].GetComponentInChildren<Text>().text = voie3[i].Name;
        }
    }

    public void Update()
    {
        if (buttonsVoie1.Any(e => e.GetComponent<ButtonWithDescription>().isMouseOn()))
        {
            for (int i = 0; i < buttonsVoie1.Length; i++)
            {
                if (buttonsVoie1[i].GetComponent<ButtonWithDescription>().isMouseOn())
                {
                    descriptionText.text = voie1[i].Description;
                    return;
                }
            }
        }
        if (buttonsVoie2.Any(e => e.GetComponent<ButtonWithDescription>().isMouseOn()))
        {
            for (int i = 0; i < buttonsVoie2.Length; i++)
            {
                if (buttonsVoie2[i].GetComponent<ButtonWithDescription>().isMouseOn())
                {
                    descriptionText.text = voie2[i].Description;
                    return;
                }
            }
        }
        if (buttonsVoie3.Any(e => e.GetComponent<ButtonWithDescription>().isMouseOn()))
        {
            for (int i = 0; i < buttonsVoie3.Length; i++)
            {
                if (buttonsVoie3[i].GetComponent<ButtonWithDescription>().isMouseOn())
                {
                    descriptionText.text = voie3[i].Description;
                    return;
                }
            }
        }
        descriptionText.text = "";
    }
    public void ChoiceWay(int id)
    {
        foreach (GameObject go in voiceGeneralSkills)
        {
            go.SetActive(false);
        }
        voiceGeneralSkills[id].SetActive(true);
    }

    public void GestionUp(int idVoie, int id)
    {
        Unit necro = GameObject.Find("Necro").GetComponent<Unit>();
        LvlData currentUp;
        if (idVoie == 1)
        {
            currentUp = voie1[id];
            GameManager.instance.SetManaMax(1);
        }
        else if (idVoie == 2)
        {
            currentUp = voie2[id];
            GameManager.instance.UpManaRegenUp(0.2f);
        }
        else
        {
            currentUp = voie3[id];
            necro.maxHealth += 5;
            necro.currentHealth += 5;
        }

        if (currentUp.IsNewSkill)
            necro.AddSkill(currentUp.IdNewSkill);

        if (currentUp.IdNewAura != 0)
            necro.ApplyStatus(currentUp.IdNewAura);

        if (currentUp.IsPersoAmelio)
        {
            //Pourcentage de up
            necro.cdAttaque *= (100f + currentUp.UpSpeedAttaque) / 100f;
            necro.range *= (100f + currentUp.UpRange) / 100f;
            necro.fieldOfView *= (100f + currentUp.UpVision) / 100f;
            necro.maxHealth = (int)(necro.maxHealth * (100f + currentUp.UpPvUnitPurcent) / 100f);
            necro.currentHealth = (int)(necro.currentHealth * (100f + currentUp.UpPvUnitPurcent) / 100f);
            necro.damage = (int)(necro.damage * (100f + currentUp.UpDamageUnitPurcent) / 100f);
            //Flat de up
            necro.defCont += currentUp.DefContandant;
            necro.defTran += currentUp.DefTranchant;
            necro.defMagi += currentUp.DefMagique;
            necro.maxHealth += currentUp.UpPvUnit;
            necro.currentHealth += currentUp.UpPvUnit;
            necro.damage += currentUp.UpDamageUnit;
            necro.speed += currentUp.UpSpeedUnit;
        }
        else
        {
            if (currentUp.UpPvUnit != 0 || currentUp.UpDamageUnit != 0 || currentUp.UpSpeedUnit != 0)
            {
                foreach (Unit oldUnit in GameManager.instance.armeeNecro.Select(e => e.GetComponent<Unit>()))
                {
                    if (oldUnit == necro)
                        continue;

                    oldUnit.maxHealth += currentUp.UpPvUnit;
                    oldUnit.currentHealth += currentUp.UpPvUnit;
                    oldUnit.damage += currentUp.UpDamageUnit;
                    oldUnit.speed += currentUp.UpSpeedUnit;
                }
                GameManager.instance.AddUpPvUnit(currentUp.UpPvUnit);
                GameManager.instance.AddUpDamageUnit(currentUp.UpDamageUnit);
                GameManager.instance.AddUpSpeedUnit(currentUp.UpSpeedUnit);
            }
            if (currentUp.UpPvUnitPurcent != 0 || currentUp.UpDamageUnitPurcent != 0)
            {
                foreach (Unit oldUnit in GameManager.instance.armeeNecro.Select(e => e.GetComponent<Unit>()))
                {
                    if (oldUnit == necro)
                        continue;
                    UniteData ud = unitBDD.Get(oldUnit.id);
                    int upPv = (int)(ud.MaxHealth * currentUp.UpPvUnitPurcent / 100f);
                    oldUnit.maxHealth += upPv;
                    oldUnit.currentHealth += upPv;
                    oldUnit.damage = (int)(ud.Damage * (100f + currentUp.UpDamageUnitPurcent) / 100f);
                }
                GameManager.instance.AddUpPvUnitPurcent(currentUp.UpPvUnitPurcent);
                GameManager.instance.AddUpDamageUnitPurcent(currentUp.UpDamageUnitPurcent);
            }
        }
        if (currentUp.UpManaMax != 0 || currentUp.UpManaRegen != 0)
        {
            GameManager.instance.SetManaMax(currentUp.UpManaMax);
            GameManager.instance.UpManaRegenUp(currentUp.UpManaRegen);
        }
    }
    public void DescriptionSkillWay1(int id)
    {
        descriptionText.text = voie1[id].Description;
    }
    public void ChoiceSkillWay1(int id)
    {
        if (p.lvlToChoice > 0 && isSelectVoie1[id])
        {
            p.lvlToChoice--;
            isSelectVoie1[id] = false;
            buttonsVoie1[id].GetComponent<Image>().color = Color.grey;
            if (id % 4 != 3)
            {
                isSelectVoie1[id + 1] = true;
                buttonsVoie1[id + 1].GetComponent<Image>().color = Color.white;
            }
            GestionUp(1, id);
        }
        p.uiPlayer.DisplayXpAndLvl();
    }
    public void ChoiceSkillWay2(int id)
    {
        if (p.lvlToChoice > 0 && isSelectVoie2[id])
        {
            p.lvlToChoice--;
            isSelectVoie2[id] = false;
            buttonsVoie2[id].GetComponent<Image>().color = Color.grey;
            if (id % 4 != 3)
            {
                isSelectVoie2[id + 1] = true;
                buttonsVoie2[id + 1].GetComponent<Image>().color = Color.white;
            }
            GestionUp(2, id);
        }
        p.uiPlayer.DisplayXpAndLvl();
    }
    public void ChoiceSkillWay3(int id)
    {
        if (p.lvlToChoice > 0 && isSelectVoie3[id])
        {
            p.lvlToChoice--;
            isSelectVoie3[id] = false;
            buttonsVoie3[id].GetComponent<Image>().color = Color.grey;
            if (id % 4 != 3)
            {
                isSelectVoie3[id + 1] = true;
                buttonsVoie3[id + 1].GetComponent<Image>().color = Color.white;
            }
            GestionUp(3, id);
        }
        p.uiPlayer.DisplayXpAndLvl();
    }

}
