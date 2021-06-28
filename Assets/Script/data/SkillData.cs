using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Perso/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Global")]
    [SerializeField] private int _id;
    public int Id { get { return _id; } }
    [SerializeField] private string _name;
    public string Name { get { return _name; } }
    [SerializeField] private string _description;
    public string Description { get { return _description; } }
    [SerializeField] private SkillType _typeSkill;
    public SkillType TypeSkill { get { return _typeSkill; } }
    [SerializeField] private int _manaCost;
    public int ManaCost { get { return _manaCost; } }
    [SerializeField] private float _timeCast;
    public float TimeCast { get { return _timeCast; } }
    [SerializeField] private int _cdTime;
    public int CdTime { get { return _cdTime; } }
    [SerializeField] private bool _isCiblageSkill;
    public bool IsCiblageSkill { get { return _isCiblageSkill; } }

    [Header("Caracteristiques sorts")]
    [SerializeField] private TypeOfCible _typeCible;
    public TypeOfCible TypeCible { get { return _typeCible; } }
    [SerializeField] private bool _isUndeadCible;
    public bool IsUndeadCible { get { return _isUndeadCible; } }
    [SerializeField] private bool _isHumansCible;
    public bool IsHumansCible { get { return _isHumansCible; } }
    [SerializeField] private bool _isAura;
    public bool IsAura { get { return _isAura; } }
    [SerializeField] private bool _isCanalisation;
    public bool IsCanalisation { get { return _isCanalisation; } }
    [SerializeField] private bool _isPurification;
    public bool IsPurification { get { return _isPurification; } }
    [SerializeField] private float _range;
    public float Range { get { return _range; } }
    [SerializeField] private float _sizeAoE;
    public float SizeAoE { get { return _sizeAoE; } }
    [SerializeField] private StatusData[] _statusArray;
    public StatusData[] StatusArray { get { return _statusArray; } }
    [SerializeField] private int _damage;
    public int Damage { get { return _damage; } }
    [SerializeField] private TypeOfDamage _typeDamage;
    public TypeOfDamage TypeDamage { get { return _typeDamage; } }
    [SerializeField] private int _heal;
    public int Heal { get { return _heal; } }
    [SerializeField] private int _moralChanger;
    public int MoralChanger { get { return _moralChanger; } }
    [SerializeField] private int _probaProc;
    public int ProbaProc { get { return _probaProc; } }

    [Header("Creation d'unites")]
    [SerializeField] private UniteData _unit;
    public UniteData Unit { get { return _unit; } }
    [SerializeField] private bool _isUndead;
    public bool IsUndead { get { return _isUndead; } }
    [SerializeField] private int _costBone;
    public int CostBone { get { return _costBone; } }
    [SerializeField] private int _costFlesh;
    public int CostFlesh { get { return _costFlesh; } }
    [SerializeField] private int _costIron;
    public int CostIron { get { return _costIron; } }
    [SerializeField] private int _costWood;
    public int CostWood { get { return _costWood; } }
    [SerializeField] private int _costFood;
    public int CostFood { get { return _costFood; } }


}
