using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class PlayerPanel : NetworkBehaviour
{
    [Header("Global")]
    [HideInInspector] public PlayerController p;
    public Text statsSelection;
    public Image imgXp;
    public Button buttonXp;
    public Button buttonXp2;

    [Header("Necro")]
    public Text manaCurrent;
    public Text manaReserve;
    public Text manaRegen;
    public Image imgCurrentMana;
    public Image imgReserveMana;
    public Image imgFondMana;
    [Header("Human")]
    public Text wood;
    public Text iron;
    public Text food;

    private void Start()
    {
        p = GetComponent<PlayerController>();
    }

    public void DisplayStats(List<string> stats)
    {
        string stringResult = "";
        if (stats != null)
        {
            foreach (string s in stats)
            {
                stringResult += $"{s}\n";
            }
            stringResult = stringResult.Substring(0, stringResult.Length - 1);
        }
        statsSelection.text = stringResult;
    }

    public void DisplayXpAndLvl()
    {
        imgXp.fillAmount = (float)p.xp / (float)(p.lvl * 50 + 100);
        buttonXp.GetComponentInChildren<Text>().text = p.lvlToChoice > 0 ? $"LVL\nUp {p.lvlToChoice}" : "Skill";
        buttonXp2.GetComponentInChildren<Text>().text = p.lvlToChoice > 0 ? $"LVL\nUp {p.lvlToChoice}" : "Skill";
    }

    #region undead
    public void SetUiForNecro()
    {
        manaCurrent.enabled = true;
        manaReserve.enabled = true;
        manaRegen.enabled = true;
        imgCurrentMana.enabled = true;
        imgReserveMana.enabled = true;
        imgFondMana.enabled = true;
        SetMana();
        SetManaReserve();
        SetManaRegen(); 
        //DisplayXpAndLvl();
    }
    public void SetMana()
    {
        manaCurrent.text = $"{Mathf.FloorToInt(GameManager.instance.mana)} / {Mathf.FloorToInt(GameManager.instance.manaMax - GameManager.instance.manaReserve)}";
        imgCurrentMana.fillAmount = (float)GameManager.instance.mana / (float)GameManager.instance.manaMax;
    }
    public void SetManaReserve()
    {
        manaReserve.text = $"{Mathf.FloorToInt(GameManager.instance.manaReserve)}";
        imgReserveMana.fillAmount = (float)GameManager.instance.manaReserve / (float)GameManager.instance.manaMax;
    }
    public void SetManaRegen()
    {
        manaRegen.text = $"{String.Format("{0:0.00}", GameManager.instance.manaRegen)}/s";
    }
    #endregion


    #region humans
    public void SetUiForHuman()
    {
        food.enabled = true;
        iron.enabled = true;
        wood.enabled = true;
        SetFood();
        SetIron();
        SetWood();
        DisplayXpAndLvl();
    }
    public void SetFood()
    {
        food.text = $"Blé : {Mathf.FloorToInt(GameManager.instance.GetFood())} - <color=red>Conso {String.Format("{0:0.00}", GameManager.instance.GetConsumtion())}/s</color>";
    }
    public void SetIron()
    {
        iron.text = $"Fer : {Mathf.FloorToInt(GameManager.instance.GetIron())}";
    }
    public void SetWood()
    {
        wood.text = $"Bois : {Mathf.FloorToInt(GameManager.instance.GetWood())}";
    }
    #endregion
}
