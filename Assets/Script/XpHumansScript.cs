using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class XpHumansScript : MonoBehaviour
{
    public PlayerController p;
    public Unit persoHero; 
    public GameObject[] voiceGeneralSkills;
    public Button[] buttonsVoie1;
    public Text descriptionText;
    [HideInInspector] public bool[] isSelectVoie1;
    public Button[] buttonsVoie2;
    [HideInInspector] public bool[] isSelectVoie2;
    public Button[] buttonsVoie3;
    [HideInInspector] public bool[] isSelectVoie3;
    public Button[] buttonsVoie4;
    [HideInInspector] public bool[] isSelectVoie4;
    public Button[] buttonsVoie5;
    [HideInInspector] public bool[] isSelectVoie5;
    public LvlData[] voie1;
    public LvlData[] voie2;
    public LvlData[] voie3;
    public LvlData[] voie4;
    public LvlData[] voie5;
    public int lvlInVoie1;
    public int lvlInVoie2;
    public int lvlInVoie3;
    public int lvlInVoie4;
    public int lvlInVoie5;
    // Start is called before the first frame update
    void Start()
    {
        isSelectVoie1 = new bool[buttonsVoie1.Length];
        isSelectVoie1[0] = true;
        buttonsVoie1[0].GetComponent<Image>().color = Color.white;
        isSelectVoie1[4] = true;
        buttonsVoie1[4].GetComponent<Image>().color = Color.white;

        isSelectVoie2 = new bool[buttonsVoie2.Length];
        isSelectVoie2[0] = true;
        buttonsVoie2[0].GetComponent<Image>().color = Color.white;
        isSelectVoie2[4] = true;
        buttonsVoie2[4].GetComponent<Image>().color = Color.white;

        isSelectVoie3 = new bool[buttonsVoie3.Length];
        isSelectVoie3[0] = true;
        buttonsVoie3[0].GetComponent<Image>().color = Color.white;
        isSelectVoie3[4] = true;
        buttonsVoie3[4].GetComponent<Image>().color = Color.white;

        isSelectVoie4 = new bool[buttonsVoie3.Length];
        isSelectVoie4[0] = true;
        buttonsVoie4[0].GetComponent<Image>().color = Color.white;
        isSelectVoie4[4] = true;
        buttonsVoie4[4].GetComponent<Image>().color = Color.white;

        isSelectVoie5 = new bool[buttonsVoie3.Length];
        isSelectVoie5[0] = true;
        buttonsVoie5[0].GetComponent<Image>().color = Color.white;
        isSelectVoie5[4] = true;
        buttonsVoie5[4].GetComponent<Image>().color = Color.white;
        int maxBoucle = Mathf.Min(buttonsVoie1.Length, voie1.Length);
        for (int i = 0; i < maxBoucle; i++)
        {
            buttonsVoie1[i].GetComponentInChildren<Text>().text = voie1[i].Name;
            buttonsVoie2[i].GetComponentInChildren<Text>().text = voie2[i].Name;
            buttonsVoie3[i].GetComponentInChildren<Text>().text = voie3[i].Name;
            buttonsVoie4[i].GetComponentInChildren<Text>().text = voie4[i].Name;
            buttonsVoie5[i].GetComponentInChildren<Text>().text = voie5[i].Name;
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
        if (buttonsVoie4.Any(e => e.GetComponent<ButtonWithDescription>().isMouseOn()))
        {
            for (int i = 0; i < buttonsVoie4.Length; i++)
            {
                if (buttonsVoie4[i].GetComponent<ButtonWithDescription>().isMouseOn())
                {
                    descriptionText.text = voie4[i].Description;
                    return;
                }
            }
        }
        if (buttonsVoie5.Any(e => e.GetComponent<ButtonWithDescription>().isMouseOn()))
        {
            for (int i = 0; i < buttonsVoie5.Length; i++)
            {
                if (buttonsVoie5[i].GetComponent<ButtonWithDescription>().isMouseOn())
                {
                    descriptionText.text = voie5[i].Description;
                    return;
                }
            }
        }
        descriptionText.text = "";
    }
    public void GestionUp(int idVoie, int id)
    {
        LvlData currentUp;
        if (idVoie == 1)
        {
            currentUp = voie1[id];
            lvlInVoie1++;
        }
        else if (idVoie == 2)
        {
            currentUp = voie2[id];
            lvlInVoie2++;
        }
        else if (idVoie == 3)
        {
            currentUp = voie3[id];
            lvlInVoie3++;
        }
        else if (idVoie == 4)
        {
            currentUp = voie4[id];
            lvlInVoie4++;
        }
        else
        {
            currentUp = voie5[id];
            lvlInVoie5++;
        }

        if (currentUp.IsNewSkill)
            p.AddSkill(persoHero, currentUp.IdNewSkill);
        if (currentUp.IsPersoAmelio)
        {
            //Pourcentage de up
            persoHero.cdAttaque *= (100f + currentUp.UpSpeedAttaque) / 100f;
            persoHero.range *= (100f + currentUp.UpRange) / 100f;
            persoHero.fieldOfView *= (100f + currentUp.UpVision) / 100f;
            persoHero.maxHealth = (int)(persoHero.maxHealth * (100f + currentUp.UpPvUnitPurcent) / 100f);
            persoHero.currentHealth = (int)(persoHero.currentHealth * (100f + currentUp.UpPvUnitPurcent) / 100f);
            persoHero.damage = (int)(persoHero.damage * (100f + currentUp.UpDamageUnitPurcent) / 100f);
            //Flat de up
            persoHero.defCont += currentUp.DefContandant;
            persoHero.defTran += currentUp.DefTranchant;
            persoHero.defMagi += currentUp.DefMagique;
            persoHero.maxHealth += currentUp.UpPvUnit;
            persoHero.currentHealth += currentUp.UpPvUnit;
            persoHero.damage += currentUp.UpDamageUnit;
            persoHero.speed += currentUp.UpSpeedUnit;
        }

        if (currentUp.UpManaMax != 0 || currentUp.UpManaRegen != 0)
        {
            GameManager.instance.SetManaMax(currentUp.UpManaMax);
            GameManager.instance.UpManaRegenUp(currentUp.UpManaRegen);
        }
    }
    public void ChoiceWay(int id)
    {
        foreach (GameObject go in voiceGeneralSkills)
        {
            go.SetActive(false);
        }
        voiceGeneralSkills[id].SetActive(true);
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
    public void ChoiceSkillWay4(int id)
    {
        if (p.lvlToChoice > 0 && isSelectVoie4[id])
        {
            p.lvlToChoice--;
            isSelectVoie4[id] = false;
            buttonsVoie4[id].GetComponent<Image>().color = Color.grey;
            if (id % 4 != 3)
            {
                isSelectVoie4[id + 1] = true;
                buttonsVoie4[id + 1].GetComponent<Image>().color = Color.white;
            }
            GestionUp(4, id);
        }
        p.uiPlayer.DisplayXpAndLvl();
    }
    public void ChoiceSkillWay5(int id)
    {
        if (p.lvlToChoice > 0 && isSelectVoie5[id])
        {
            p.lvlToChoice--;
            isSelectVoie5[id] = false;
            buttonsVoie5[id].GetComponent<Image>().color = Color.grey;
            if (id % 4 != 3)
            {
                isSelectVoie5[id + 1] = true;
                buttonsVoie5[id + 1].GetComponent<Image>().color = Color.white;
            }
            GestionUp(5, id);
        }
        p.uiPlayer.DisplayXpAndLvl();
    }
}
