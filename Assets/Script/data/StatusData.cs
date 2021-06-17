using UnityEngine;

[CreateAssetMenu(fileName = "NewStatus", menuName = "Perso/Status")]
public class StatusData : ScriptableObject
{
    [Header("Global")]
    [SerializeField] private int _id;
    public int Id { get { return _id; } }
    [SerializeField] private string _name;
    public string Name { get { return _name; } }
    [SerializeField] private string _description;
    public string Description { get { return _description; } }
    [SerializeField] private bool _isNegatif;
    public bool IsNegatif { get { return _isNegatif; } }

    [Header("Stats")]
    [SerializeField] private bool _isCanalisation;
    public bool IsCanalisation { get { return _isCanalisation; } }
    [SerializeField] private float _timerLife;
    public float TimerLife { get { return _timerLife; } }
    [SerializeField] private bool _isProc;
    public bool IsProc { get { return _isProc; } }
    [SerializeField] private float _timerProc;
    public float TimerProc { get { return _timerProc; } }
    [SerializeField] private bool _isAura;
    public bool IsAura { get { return _isAura; } }
    [SerializeField] private bool _isTargetUndead;
    public bool IsTargetUndead { get { return _isTargetUndead; } }
    [SerializeField] private int _speedProdValue;
    public int SpeedProdValue { get { return _speedProdValue; } }
    [SerializeField] private int _speedMoveValue;
    public int SpeedMoveValue { get { return _speedMoveValue; } }
    [SerializeField] private int _speedAttaque;
    public int SpeedAttaque { get { return _speedAttaque; } }
    [SerializeField] private int _defContandant;
    public int DefContandant { get { return _defContandant; } }
    [SerializeField] private int _defTranchant;
    public int DefTranchant { get { return _defTranchant; } }
    [SerializeField] private int _defMagique;
    public int DefMagique { get { return _defMagique; } }
    [SerializeField] private int _boostDamage;
    public int BoostDamage { get { return _boostDamage; } }
    [SerializeField] private bool _isLifeSteal;
    public bool IsLifeSteal { get { return _isLifeSteal; } }
    [SerializeField] private int _damage;
    public int Damage { get { return _damage; } }
    [SerializeField] private int _heal;
    public int Heal { get { return _heal; } }
    [SerializeField] private int  _damageAround;
    public int DamageAround { get { return _damageAround; } }
    [SerializeField] private TypeOfDamage _typeDamage;
    public TypeOfDamage TypeDamage { get { return _typeDamage; } }
    [SerializeField] private int _moralModificateur;
    public int MoralModificateur { get { return _moralModificateur; } }
    [SerializeField] private bool _isInvisible;
    public bool IsInvisible { get { return _isInvisible; } }



}
