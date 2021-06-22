using System;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;
    [SerializeField] private Transform skillCircleTransform;
    [SerializeField] private Transform returnClicTransform;
    [SyncVar] public bool isAssign;
    public UniteBDD unitBDD;
    public float speed;
    public float tickIncantation;
    private Vector3 startPosition;
    private bool isCadreSelectionEnCours;
    private bool isSkillTargetEnCours;
    public Canvas canvasPlayer;
    public Canvas canvasXp;
    public Button[] actionsButtons;
    public float xp;
    [HideInInspector] public int lvl;
    [SyncVar] public int lvlToChoice;
    public PlayerPanel uiPlayer;
    public XpHumansScript xpPlayer;
    public List<Unit> unitsSelected;
    [HideInInspector] public List<Village> villagesSelected;
    public List<Skills> skillsList;
    public List<Unit> unitsSelectedForKeyboardList;
    public List<int> idUnitsSelectedForKeyboardList;
    private List<int> unitsIdSelected;
    public CursorMode cursorMode = CursorMode.Auto;
    public Texture2D defaultTexture;
    public Texture2D attackTexture;
    public Texture2D moveTexture;
    public Texture2D castSkillTexture;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Cursor.SetCursor(defaultTexture, Vector2.zero, cursorMode);
        skillsList = new List<Skills>();
        unitsSelected = new List<Unit>();
        unitsSelectedForKeyboardList = new List<Unit>();
        unitsIdSelected = new List<int>();
        idUnitsSelectedForKeyboardList = new List<int>();
        if (isLocalPlayer)
        {
            if (gameObject.CompareTag("Humans"))
            {
                Debug.Log("je passe ici");
                canvasXp = transform.GetComponentInChildren<XpHumansScript>().transform.GetComponent<Canvas>();
            }
            selectionAreaTransform.gameObject.SetActive(true);
            skillCircleTransform.gameObject.SetActive(true);
            returnClicTransform.gameObject.SetActive(true);
            GetComponentInChildren<SpriteRenderer>().enabled = true;
            canvasPlayer.enabled = true;
        }
        yield return new WaitForSeconds(0.5f);

        if (isLocalPlayer && gameObject.CompareTag("Humans") && GameObject.Find("MasterFoW") != null)
            GameObject.Find("MasterFoW").GetComponent<FogOfWarMaster>().isNecro = false;
    }

    private void Update()
    {
        if (xp >= lvl * 50 + 100)
        {
            xp -= lvl * 50 + 100;
            lvl++;
            lvlToChoice++;
            uiPlayer.DisplayXpAndLvl();
        }
        if (tickIncantation == 0)
        {
            GestionSkillKeyBoard();
            MouvCamByMouse();
            ManageCamera();
            ManageSelection();
            if (Input.GetMouseButtonDown(1))
            {
                if (unitsSelected.Count > 0 && unitsSelected.Any(e => e != null))
                {
                    MoveUnits();
                    StartCoroutine(DoReturnOnClic());
                }
            }
            if (gameObject.CompareTag("Undead"))
            {
                //if (Input.GetKeyDown(KeyCode.A))
                //    ReleverLesCadavres();
                if (Input.GetKeyDown(KeyCode.T))
                {
                    TourneAPopInAcolyte();
                    SacrificePopulation();
                }
            }
            if (gameObject.CompareTag("Humans"))
            {
                if (Input.GetKeyDown(KeyCode.T))
                    CreateConvoi();
            }
        }
        else if (Time.time >= tickIncantation)
        {
            tickIncantation = 0;
        }
        if (unitsSelected.Count > 0)
            uiPlayer.DisplayStats(unitsSelected[0].Stats());
    }
    public void GestionSkillKeyBoard()
    {
        int i;
        if (Input.GetKeyDown(KeyCode.A))
            i = 0;
        else if (Input.GetKeyDown(KeyCode.Z))
            i = 1;
        else if (Input.GetKeyDown(KeyCode.E))
            i = 2;
        else if (Input.GetKeyDown(KeyCode.R))
            i = 3;
        else if (Input.GetKeyDown(KeyCode.Q))
            i = 4;
        else if (Input.GetKeyDown(KeyCode.S))
            i = 5;
        else if (Input.GetKeyDown(KeyCode.D))
            i = 6;
        else if (Input.GetKeyDown(KeyCode.F))
            i = 7;
        else if (Input.GetKeyDown(KeyCode.W))
            i = 8;
        else if (Input.GetKeyDown(KeyCode.X))
            i = 9;
        else if (Input.GetKeyDown(KeyCode.C))
            i = 10;
        else if (Input.GetKeyDown(KeyCode.V))
            i = 11;
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(TriggerAction(unitsSelected, ActionUnite.mouv));
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(TriggerAction(unitsSelected, ActionUnite.mouvAttak));
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (Unit u in unitsSelected)
            {
                u.ChangeAction(ActionUnite.idle, Vector3.zero);
            }
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (Unit u in unitsSelected.Where(e => e.isRecolteur))
            {
                u.ChangeAction(ActionUnite.work, Vector3.zero);
            }
            return;
        }
        else
            return;
        if (unitsSelectedForKeyboardList.Count > i)
            StartCoroutine(TriggerButton(unitsSelectedForKeyboardList[i], idUnitsSelectedForKeyboardList[i]));
    }

    public void AddSkill(Unit persoHero, int idNewSkill)
    {
        persoHero.AddNewSkill(idNewSkill);
    }

    public void MoveUnits()
    {
        Vector3 v3clic = PositionMouse(new Vector3(0, 1.2f, 0));
        int j = 0;
        foreach (Unit unit in unitsSelected.Where(e => e != null))
        {
            Vector3 decal = DecalForMove(j);
            SetTargetToUnit(unit, v3clic + decal);
            j++;
        }
    }
    public Vector3 DecalForMove(int numberOfSelection)
    {
        if (numberOfSelection == 0)
            return Vector3.zero;
        int rang = 1;
        while (numberOfSelection > rang * 6)
        {
            numberOfSelection -= rang * 6;
            rang++;
        }
        return Quaternion.AngleAxis(60 * numberOfSelection / rang, Vector3.up) * Vector3.left * (rang * 2);
    }
    public void ValideCreateCharette(GameObject villageTarget)
    {
        GameManager.instance.CostAndCreateCharette(4, 4, GameObject.Find("Castle").GetComponent<Unit>(), GameObject.Find("Castle").transform.position, villageTarget);
    }

    public void ChangeUIRessources()
    {
        if (gameObject.CompareTag("Undead"))
        {
            uiPlayer.SetMana();
            uiPlayer.SetManaRegen();
            uiPlayer.SetManaReserve();
        }
        else
        {
            uiPlayer.SetFood();
            uiPlayer.SetIron();
            uiPlayer.SetWood();
        }
    }

    public void SetTargetToUnit(Unit u, Vector3 pos)
    {
        u.ChangeAction(ActionUnite.mouv, pos);
    }

    public void ManageSelection()
    {
        if (isLocalPlayer)
        {

            if (Input.GetMouseButtonDown(0) && !MouseInUi() && !isSkillTargetEnCours)
            {
                selectionAreaTransform.gameObject.SetActive(true);
                startPosition = PositionMouse();
                isCadreSelectionEnCours = true;
                ClearSkillOfSelection();
            }
            if (Input.GetMouseButton(0) && isCadreSelectionEnCours)
            {
                Vector3 currentPosition = PositionMouse();
                Vector3 leftBot = new Vector3(
                    Mathf.Min(startPosition.x, currentPosition.x),
                    Mathf.Min(startPosition.y + 0.2f, currentPosition.y + 0.2f),
                    Mathf.Min(startPosition.z, currentPosition.z)
                    );
                Vector3 rightTop = new Vector3(
                    Mathf.Max(startPosition.x, currentPosition.x),
                    Mathf.Max(startPosition.y + 0.2f, currentPosition.y + 0.2f),
                    Mathf.Max(startPosition.z, currentPosition.z)
                    );
                selectionAreaTransform.position = leftBot;
                selectionAreaTransform.localScale = rightTop - leftBot;
            }

            if (Input.GetMouseButtonUp(0) && isCadreSelectionEnCours)
            {
                isCadreSelectionEnCours = false;
                selectionAreaTransform.gameObject.SetActive(false);
                foreach (Unit unit in unitsSelected)
                {
                    if (unit != null)
                        unit.Selected(false);
                }
                unitsSelected.Clear();
                foreach (Village village in villagesSelected)
                {
                    if (village != null)
                        village.Selected(false);
                }
                unitsSelected.Clear();
                villagesSelected.Clear();
                Vector3 centerOfBox = (startPosition + PositionMouse()) / 2;
                Vector3 rayonOfBox = (startPosition - PositionMouse()) / 2;
                rayonOfBox = new Vector3(Mathf.Abs(rayonOfBox.x), 50, Mathf.Abs(rayonOfBox.z));
                Collider[] colliderArray = Physics.OverlapBox(centerOfBox, rayonOfBox);
                foreach (Collider col in colliderArray)
                {
                    Unit unitOnSelect = col.GetComponentInParent<Unit>();
                    if (unitOnSelect != null && unitOnSelect.CompareTag(gameObject.tag) && !unitOnSelect.isTransport)
                    {
                        unitsSelected.Add(unitOnSelect);
                        unitOnSelect.Selected(true);
                        ManageSkillOfSelection(unitsSelected.Count - 1);
                    }
                    Village villageOnSelect = col.GetComponentInParent<Village>();
                    if (villageOnSelect != null)
                    {
                        villagesSelected.Add(villageOnSelect);
                        villageOnSelect.Selected(true);
                    }
                    //Envoi du texte de stats pour le UI
                    if (unitsSelected.Count > 0)
                        uiPlayer.DisplayStats(unitsSelected[0].Stats());
                    else if (villagesSelected.Count > 0)
                        uiPlayer.DisplayStats(villagesSelected[0].Stats(CompareTag("Undead")));
                    else
                        uiPlayer.DisplayStats(null);
                }
            }
        }
    }
    public void ClearSkillOfSelection()
    {
        for (int i = 0; i < actionsButtons.Length; i++)
        {
            actionsButtons[i].onClick.RemoveAllListeners();
            string raccourci;
            switch (i)
            {
                case 0:
                    raccourci = "A";
                    break;
                case 1:
                    raccourci = "Z";
                    break;
                case 2:
                    raccourci = "E";
                    break;
                case 3:
                    raccourci = "R";
                    break;
                case 4:
                    raccourci = "Q";
                    break;
                case 5:
                    raccourci = "S";
                    break;
                case 6:
                    raccourci = "D";
                    break;
                case 7:
                    raccourci = "F";
                    break;
                case 8:
                    raccourci = "W";
                    break;
                case 9:
                    raccourci = "X";
                    break;
                case 10:
                    raccourci = "C";
                    break;
                case 11:
                    raccourci = "V";
                    break;
                default:
                    raccourci = "A";
                    break;
            }
            actionsButtons[i].GetComponentInChildren<Text>().text = raccourci;
        }
        skillsList.Clear();
        unitsIdSelected.Clear();
        unitsSelectedForKeyboardList.Clear();
        idUnitsSelectedForKeyboardList.Clear();
    }
    public void ManageSkillOfSelection(int lastUnitSelected)
    {
        if (!unitsIdSelected.Any(u => u == unitsSelected[lastUnitSelected].id))
        {
            unitsIdSelected.Add(unitsSelected[lastUnitSelected].id);
            if (unitsSelected[lastUnitSelected].skillsList != null && unitsSelected[lastUnitSelected].skillsList.Count > 0)
            {
                for (int i = 0; i < unitsSelected[lastUnitSelected].skillsList.Count; i++)
                {
                    int interm = i;
                    Unit unitInterm = unitsSelected[lastUnitSelected];
                    skillsList.Add(unitsSelected[lastUnitSelected].skillsList[i]);
                    unitsSelectedForKeyboardList.Add(unitsSelected[lastUnitSelected]);
                    idUnitsSelectedForKeyboardList.Add(i);
                    actionsButtons[i].onClick.AddListener(() => StartCoroutine(TriggerButton(unitInterm, interm)));
                    actionsButtons[i].GetComponentInChildren<Text>().text = skillsList[i].nameSkill;
                }
            }
        }
    }

    public void AssignXpPanelToHero(Unit unit)
    {
        canvasXp = transform.GetComponentInChildren<XpHumansScript>().transform.GetComponent<Canvas>();
        GetComponentInChildren<XpHumansScript>().persoHero = unit;
    }

    /// <summary>
    /// Permet de mouv clic ou attaqueclic
    /// </summary>
    public IEnumerator TriggerAction(List<Unit> units, ActionUnite act)
    {
        if (act == ActionUnite.mouv)
            Cursor.SetCursor(moveTexture, Vector2.zero, cursorMode);
        else if (act == ActionUnite.mouvAttak)
            Cursor.SetCursor(attackTexture, Vector2.zero, cursorMode);
        isSkillTargetEnCours = true;
        yield return DoWaitForClic();
        Cursor.SetCursor(defaultTexture, Vector2.zero, cursorMode);
        int j = 0;
        foreach (Unit u in units)
        {
            Vector3 v3clic = PositionMouse(new Vector3(0, 1.2f, 0));
            Vector3 decal = DecalForMove(j);
            u.ChangeAction(act, PositionMouse() + decal);
            j++;
        }
        isSkillTargetEnCours = false;
        StartCoroutine(DoReturnOnClic());
    }
    /// <summary>
    /// Permet de déclencher le ième skill de l'unité selectionné
    /// </summary>
    public IEnumerator TriggerButton(Unit unit, int persoIdSkill)
    {
        Cursor.SetCursor(castSkillTexture, Vector2.zero, cursorMode);
        if (unit.skillsList[persoIdSkill].isCiblable)
        {
            isSkillTargetEnCours = true;
            if (unit.skillsList[persoIdSkill].sizeAoE > 0)
            {
                GestionSkillArea(unit.skillsList[persoIdSkill].sizeAoE);
                skillCircleTransform.localScale = new Vector3(unit.skillsList[persoIdSkill].sizeAoE, 1, unit.skillsList[persoIdSkill].sizeAoE);
                skillCircleTransform.gameObject.SetActive(true);
            }
            yield return DoWaitForClic(unit.skillsList[persoIdSkill].sizeAoE);
            skillCircleTransform.gameObject.SetActive(false);
            isSkillTargetEnCours = false;
            StartCoroutine(DoReturnOnClic());
            unit.TriggerSkill(persoIdSkill, PositionMouse());
        }
        else
            unit.TriggerSkill(persoIdSkill, unit.transform.position);
        Cursor.SetCursor(defaultTexture, Vector2.zero, cursorMode);
    }

    IEnumerator DoWaitForClic(float area = 0)
    {
        while (!Input.GetMouseButtonDown(0) || MouseInUi())
        {
            if (area != 0)
                GestionSkillArea(area);
            yield return null;
        }
    }

    IEnumerator DoReturnOnClic()
    {
        returnClicTransform.transform.position = PositionMouse();
        if (!isSkillTargetEnCours)
        {
            returnClicTransform.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            returnClicTransform.gameObject.SetActive(false);
        }
    }

    public void GestionSkillArea(float area)
    {
        skillCircleTransform.transform.position = PositionMouse() - new Vector3(area / 2, 0, area / 2);

    }
    public void MouvCamByMouse()
    {
        //float deltaX = 0;
        //float deltaZ = 0;
        //if (Input.mousePosition.y / Screen.height < 0.01f)
        //    deltaZ = -1;
        //if (Input.mousePosition.y / Screen.height > 0.99f)
        //    deltaZ = 1;
        //if (Input.mousePosition.x / Screen.width < 0.01f)
        //    deltaX = -1;
        //if (Input.mousePosition.x / Screen.width > 0.99f)
        //    deltaX = 1;
        //Vector3 deplacementCam = new Vector3(deltaX, 0, deltaZ).normalized;
        //transform.Translate(deplacementCam * speed * Time.fixedDeltaTime, 0);
    }
    public bool MouseInUi()
    {
        return (Input.mousePosition.y / Screen.height) < 0.23f;
    }
    public Vector3 PositionMouse()
    {
        RaycastHit hit;
        var ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Vector3 result = Vector3.zero;
        if (Physics.Raycast(ray, out hit))
        {
            result = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
        return result;
    }
    public Vector3 PositionMouse(Vector3 offset)
    {
        RaycastHit hit;
        var ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Vector3 result = Vector3.zero;
        if (Physics.Raycast(ray, out hit))
        {
            result = new Vector3(hit.point.x - offset.x, hit.point.y - offset.y, hit.point.z - offset.z);
        }
        return result;
    }

    [Command]
    public void CmdCreateUnit(int idUnit, Vector3 position)
    {
        GameManager.instance.RpcCreateUnit(idUnit, position);
    }
    public void TourneAPopInAcolyte()
    {
        if (unitsSelected.Any(e => e.name == "Necro"))
        {
            GameObject necro = unitsSelected.First(e => e.name == "Necro").gameObject;
            GameObject village = GameObject.FindGameObjectsWithTag("Village").OrderBy(e => (e.transform.position - necro.transform.position).magnitude).First(e => e.GetComponent<Village>() != null);
            if (GameManager.instance.mana >= 10 && (village.transform.position - necro.transform.position).magnitude < 15)
            {
                GameManager.instance.mana -= 10;
                tickIncantation = 1f + Time.time;
                StartCoroutine(TurnAPaysanInAcolyte(village));
            }
        }
    }

    public void GestionXpUp(XpOnVoie xpOnVoie, float xpUp)
    {
        if (!isLocalPlayer || xpPlayer == null)
            return;
        switch (xpOnVoie)
        {
            case XpOnVoie.Magistrat:
                xpUp *= (1f + (xpPlayer.lvlInVoie1 / 2f));
                break;
            case XpOnVoie.Eclaireur:
                xpUp *= (1f + (xpPlayer.lvlInVoie2 / 2f));
                break;
            case XpOnVoie.Martial:
                xpUp *= (1f + (xpPlayer.lvlInVoie3 / 2f));
                break;
            case XpOnVoie.Lumiere:
                xpUp *= (1f + (xpPlayer.lvlInVoie4 / 2f));
                break;
            case XpOnVoie.Mage:
                xpUp *= (1f + (xpPlayer.lvlInVoie5 / 2f));
                break;
        }
        xp += xpUp;
        uiPlayer.DisplayXpAndLvl();

    }
    public void GestionXpUpNecro(float xpUp)
    {
        if (!isLocalPlayer || xpPlayer == null)
            return;

        xp += xpUp;
        uiPlayer.DisplayXpAndLvl();

    }
    public void SacrificePopulation()
    {
        if (villagesSelected.Count != 0)
        {
            foreach (Village village in villagesSelected)
            {
                if (village.popAcolyte.Count > 0 && village.popPaysan.Count > village.popAcolyte.Count)
                {
                    village.SacrificeAcolyteToKill();
                }
            }
        }
    }
    public void CanvasLvl(bool isForOpen)
    {
        canvasXp.enabled = isForOpen;
    }
    public void CreateConvoi()
    {
        GetComponentInChildren<TransportCreation>().InitGui();
    }
    public IEnumerator TurnAPaysanInAcolyte(GameObject village)
    {
        yield return new WaitForSeconds(2f);
        village.GetComponent<Village>().CorruptAcolyte();
        GameManager.instance.CountRegenerationMana();
    }
    void ManageCamera()
    {
        float deltaX = Input.GetAxisRaw("Horizontal");
        float deltaZ = Input.GetAxisRaw("Vertical");
        Vector3 deplacementCam = new Vector3(deltaX, 0, deltaZ).normalized;
        transform.Translate(deplacementCam * speed * Time.fixedDeltaTime, 0);

    }
}
