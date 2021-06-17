

public enum Ressources
{
    wood,
    iron,
    food
}

public enum ActionUnite
{
    idle,
    mouv,
    mouvAttak,
    attack,
    work,
}

public enum TypeOfDamage
{
    contandant,
    tranchant,
    magique,
    brut,
}
public enum SkillType
{
    CreateUnitUD,
    CreateUnitHU,
    RaiseZombie,
    CorruptPaysan,
    SkillAttack,
    Purge,
}
public enum TypeOfCible
{
    units,
    charette,
    bone,
    ground,
    village,
    batiment,
}
public enum StatMoral
{
    fleeing,
    scared,
    normal,
    determined,
}
public enum LevelingTarget
{
    units, // ex : pv des unites
    hero, // ex : pv du necro
    skill, // ex : ameliorations de sorts de creation
    playercontroller, // ex : mana du necro
}
public enum XpOnVoie
{
    Magistrat,
    Eclaireur,
    Martial,
    Lumiere,
    Mage,
}