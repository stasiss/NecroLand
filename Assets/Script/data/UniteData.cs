using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnite", menuName = "Perso/Unite")]
public class UniteData : ScriptableObject
{
    [Header("Global")]
    [SerializeField] private int _id;
    public int Id { get { return _id; } }
    [SerializeField] private string _name;
    public string Name { get { return _name; } }
    [SerializeField] private GameObject _prefab;
    public GameObject Prefab { get { return _prefab; } }
    [SerializeField] private bool _isUndead;
    public bool IsUndead { get { return _isUndead; } }
    [SerializeField] private int _idCorpse;
    public int IdCorpse { get { return _idCorpse; } }
    [SerializeField] private int _probaCorpse;
    public int ProbaCorpse { get { return _probaCorpse; } }
    [SerializeField] private int _bone;
    public int Bone { get { return _bone; } }
    [SerializeField] private int _flesh;
    public int Flesh { get { return _flesh; } }
    [SerializeField] private GameObject _projectile;
    public GameObject Projectile { get { return _projectile; } }


    [Header("Statistiques")]
    [SerializeField] private int _maxHealth;
    public int MaxHealth { get { return _maxHealth; } }
    [SerializeField] private TypeOfDamage _typeDamage;
    public TypeOfDamage TypeDamage { get { return _typeDamage; } }
    [SerializeField] private int _damage;
    public int Damage { get { return _damage; } }
    [SerializeField] private int _defContandant;
    public int DefContandant { get { return _defContandant; } }
    [SerializeField] private int _defTranchant;
    public int DefTranchant { get { return _defTranchant; } }
    [SerializeField] private int _defMagique;
    public int DefMagique { get { return _defMagique; } }
    [SerializeField] private float _speed;
    public float Speed { get { return _speed; } }
    [SerializeField] private float _range;
    public float Range { get { return _range; } }
    [SerializeField] private float _cdAttack;
    public float CdAttack { get { return _cdAttack; } }
    [SerializeField] private float _fieldOfView;
    public float FieldOfView { get { return _fieldOfView; } }
    [SerializeField] private float _manaReserve;
    public float ManaReserve { get { return _manaReserve; } }
    [SerializeField] private float _foodComsomption;
    public float FoodComsomption { get { return _foodComsomption; } }
    [SerializeField] private List<SkillData> _skillsList;
    public List<SkillData> SkillsList { get { return _skillsList; } }
    [SerializeField] private bool _isInvisible;
    public bool IsInvisible { get { return _isInvisible; } }
    [SerializeField] private bool _isDetecteur;
    public bool IsDetecteur { get { return _isDetecteur; } }

    [Header("Moral")]
    [SerializeField] private int _middleMoral;
    public int MiddleMoral { get { return _middleMoral; } }
    [SerializeField] private int _maxMoral;
    public int MaxMoral { get { return _maxMoral; } }
    [SerializeField] private int _seuilFuiteMoral;
    public int SeuilFuiteMoral { get { return _seuilFuiteMoral; } }
    [SerializeField] private int _seuilBasMoral;
    public int SeuilBasMoral { get { return _seuilBasMoral; } }
    [SerializeField] private int _seuilHautMoral;
    public int SeuilHautMoral { get { return _seuilHautMoral; } }

    [Header("Transport")]
    [SerializeField] private bool _isTransport;
    public bool IsTransport { get { return _isTransport; } }
    [SerializeField] private int _maxStock;
    public int MaxStock { get { return _maxStock; } }

    [Header("Recolteur")]
    [SerializeField] private bool _isRecolteur;
    public bool IsRecolteur { get { return _isRecolteur; } }
    [SerializeField] private bool _isCorpseRecolteur;
    public bool IsCorpseRecolteur { get { return _isCorpseRecolteur; } }
    [Header("Piege")]
    [SerializeField] private bool _isTrap;
    public bool IsTrap{ get { return _isTrap; } }
    [SerializeField] private float _timerLife;
    public float TimerLife { get { return _timerLife; } }

}
