using UnityEngine;

[CreateAssetMenu(fileName = "NewLvl", menuName = "Perso/LvlUp")]
public class LvlData : ScriptableObject
{
    [Header("Global")]
    [SerializeField] private int _id;
    public int Id { get { return _id; } }
    [SerializeField] private string _name;
    public string Name { get { return _name; } }
    [SerializeField] private string _description;
    public string Description { get { return _description; } }
    [SerializeField] private int _voie;
    public int Voie { get { return _voie; } }
    [SerializeField] private bool _isUndead;
    public bool IsUndead { get { return _isUndead; } }

    [Header("LvlController")]
    [SerializeField] private int _upManaMax;
    public int UpManaMax { get { return _upManaMax; } }
    [SerializeField] private float _upManaRegen;
    public float UpManaRegen { get { return _upManaRegen; } }

    [Header("Statistiques perso")]
    [SerializeField] private bool _isPersoAmelio;
    public bool IsPersoAmelio { get { return _isPersoAmelio; } }
    [SerializeField] private bool _isNewSkill;
    public bool IsNewSkill { get { return _isNewSkill; } }
    [SerializeField] private int _idNewSkill;
    public int IdNewSkill { get { return _idNewSkill; } }
    [SerializeField] private int _idNewAura;
    public int IdNewAura { get { return _idNewAura; } }
    [SerializeField] private float _upSpeedAttaque;
    public float UpSpeedAttaque { get { return _upSpeedAttaque; } }
    [SerializeField] private int _upRange;
    public int UpRange { get { return _upRange; } }
    [SerializeField] private int _upVision;
    public int UpVision { get { return _upVision; } }
    [SerializeField] private int _defContandant;
    public int DefContandant { get { return _defContandant; } }
    [SerializeField] private int _defTranchant;
    public int DefTranchant { get { return _defTranchant; } }
    [SerializeField] private int _defMagique;
    public int DefMagique { get { return _defMagique; } }

    [Header("Statistiques unite")]
    [SerializeField] private int _upPvUnit;
    public int UpPvUnit { get { return _upPvUnit; } }
    [SerializeField] private int _upPvUnitPurcent;
    public int UpPvUnitPurcent { get { return _upPvUnitPurcent; } }
    [SerializeField] private float _upSpeedUnit;
    public float UpSpeedUnit { get { return _upSpeedUnit; } }
    [SerializeField] private int _upDamageUnit;
    public int UpDamageUnit { get { return _upDamageUnit; } }
    [SerializeField] private int _upDamageUnitPurcent;
    public int UpDamageUnitPurcent { get { return _upDamageUnitPurcent; } }

    [Header("Modification unit")]
    [SerializeField] private int _idUnitBuff;
    public int IdUnitBuff { get { return _idUnitBuff; } }
    [SerializeField] private int[] _idMultiUnitBuff;
    public int[] IdMultiUnitBuff { get { return _idMultiUnitBuff; } }

    [Header("Modification skills")]
    [SerializeField] private int _idSkillBuff;
    public int IdSkillBuff { get { return _idSkillBuff; } }

}
