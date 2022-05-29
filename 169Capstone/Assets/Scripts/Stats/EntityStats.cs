using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    // Values that can be Secondary Line Values
    HitPoints,
    AttackSpeed,
    MoveSpeed,
    Defense,
    DodgeChance,
    CritChance,
    CritDamage,
    TrapDamageResist,
    Haste,

    // ONLY Primary Lines
    STRDamage,
    DEXDamage,
    INTDamage,
    WISDamage,

    enumSize
}

public abstract class EntityStats : MonoBehaviour
{
    public const int numberOfSecondaryLineOptions = 9;

    protected virtual void Awake()
    {
        statBonusFromSource = new Dictionary<Object, Dictionary<StatBonusType, float>>();
    }

    #region Bonus Management
        
        public enum BonusType
        {
            flat,
            multiplier,

            enumSize
        }

        protected struct StatBonusType : System.IEquatable<StatBonusType>
        {
            public StatType stat;
            public BonusType bonus;

            public StatBonusType(StatType s, BonusType b)
            {
                stat = s;
                bonus = b;
            }

            public override bool Equals(object obj) => obj is StatBonusType other && this.Equals(other);

            public bool Equals(StatBonusType other)
            {
                return stat == other.stat && bonus == other.bonus;
            }

            public override int GetHashCode() => ((int)stat, (int)bonus).GetHashCode();
        }

        protected Dictionary<Object, Dictionary<StatBonusType, float>> statBonusFromSource;

        public virtual void SetBonusForStat(Object bonusSource, StatType stat, BonusType bonusType, float bonusAmount)
        {
            if(bonusSource == null || stat == StatType.enumSize ||  bonusType == BonusType.enumSize)
                return;
            
            if(statBonusFromSource.ContainsKey(bonusSource))
            {
                if(bonusAmount == 0)
                {
                    statBonusFromSource.Remove(bonusSource);
                    RecalculateStatBonus(stat, bonusType);
                    return;
                }
            }
            else
            {
                statBonusFromSource.Add(bonusSource, new Dictionary<StatBonusType, float>());
            }

            StatBonusType sbt = new StatBonusType(stat, bonusType);
            if(statBonusFromSource[bonusSource].ContainsKey(sbt))
            {
                statBonusFromSource[bonusSource][sbt] = bonusAmount;
            }
            else
            {
                statBonusFromSource[bonusSource].Add(sbt, bonusAmount);
            }

            RecalculateStatBonus(stat, bonusType);
        }

        public virtual float? GetBonusForStat(Object bonusSource, StatType stat, BonusType bonusType)
        {
            if(bonusSource == null || stat == StatType.enumSize ||  bonusType == BonusType.enumSize)
                return null;
            
            if(!statBonusFromSource.ContainsKey(bonusSource))
                return null;

            StatBonusType sbt = new StatBonusType(stat, bonusType);
            if(statBonusFromSource[bonusSource].ContainsKey(sbt))
                return statBonusFromSource[bonusSource][sbt];
            else
                return null;
        }

        protected virtual void RecalculateStatBonus(StatType stat, BonusType bonusType)
        {
            if(stat == StatType.enumSize ||  bonusType == BonusType.enumSize)
                return;

            float total = bonusType == BonusType.flat ? 0 : 1;
            StatBonusType sbt = new StatBonusType(stat, bonusType);
            foreach(Object key in new List<Object>(statBonusFromSource.Keys))
            {
                if(key == null)
                {
                    statBonusFromSource.Remove(key);
                }
                else
                {
                    if(statBonusFromSource[key].ContainsKey(sbt))
                        total += statBonusFromSource[key][sbt];
                }
            }

            switch(stat)
            {
                case StatType.HitPoints:
                    if(bonusType == BonusType.flat)
                        maxHitPointsFlatBonus = total;
                    else
                        maxHitPointsMultiplier = total;
                    break;

                case StatType.AttackSpeed:
                    if(bonusType == BonusType.flat)
                        attackSpeedFlatBonus = total;
                    else
                        attackSpeedMultiplier = total;
                    break;

                case StatType.MoveSpeed:
                    if(bonusType == BonusType.flat)
                        moveSpeedFlatBonus = total;
                    else
                        moveSpeedMultiplier = total;
                    break;

                case StatType.Defense:
                    if(bonusType == BonusType.flat)
                        defenseFlatBonus = total;
                    else
                        defenseMultiplier = total;
                    break;
                    
                case StatType.DodgeChance:
                    if(bonusType == BonusType.flat)
                        dodgeChanceFlatBonus = total;
                    break;

                case StatType.CritChance:
                    if(bonusType == BonusType.flat)
                        critChanceFlatBonus = total;
                    break;

                case StatType.CritDamage:
                    if(bonusType == BonusType.flat)
                        critDamageFlatBonus = total;
                    else
                        critDamageMultiplier = total;
                    break;

                case StatType.TrapDamageResist:
                    if(bonusType == BonusType.flat)
                        trapDamageResistFlatBonus = total;
                    else
                        trapDamageResistMultiplier = total;
                    break;

                case StatType.Haste:
                    if(bonusType == BonusType.flat)
                        hasteFlatBonus = total;
                    break;

                case StatType.STRDamage:
                    if(bonusType == BonusType.flat)
                        STRDamageFlatBonus = total;
                    else
                        STRDamageMultiplier = total;
                    break;

                case StatType.DEXDamage:
                    if(bonusType == BonusType.flat)
                        DEXDamageFlatBonus = total;
                    else
                        DEXDamageMultiplier = total;
                    break;

                case StatType.WISDamage:
                    if(bonusType == BonusType.flat)
                        WISDamageFlatBonus = total;
                    else
                        WISDamageMultiplier = total;
                    break;

                case StatType.INTDamage:
                    if(bonusType == BonusType.flat)
                        INTDamageFlatBonus = total;
                    else
                        INTDamageMultiplier = total;
                    break;
            }
        }
    #endregion

    #region Player Stat Values
        // Need to be stored here due to enum & bonus management reasons
        protected float hasteBase;
        protected float hasteFlatBonus;

        public float STRDamageFlatBonus {get; protected set;}
        public float STRDamageMultiplier {get; protected set;}

        public float DEXDamageFlatBonus {get; protected set;}
        public float DEXDamageMultiplier {get; protected set;}
        
        public float WISDamageFlatBonus {get; protected set;}
        public float WISDamageMultiplier {get; protected set;}
        
        public float INTDamageFlatBonus {get; protected set;}
        public float INTDamageMultiplier {get; protected set;}
    #endregion

    #region Hit Points
        protected float maxHitPointsBase;
        protected float maxHitPointsMultiplier;
        protected float maxHitPointsFlatBonus;

        public virtual float getMaxHitPoints()
        {
            return maxHitPointsBase * maxHitPointsMultiplier + maxHitPointsFlatBonus;
        }
    #endregion

    #region Attack Speed
        protected float attackSpeedBase;
        protected float attackSpeedMultiplier;
        protected float attackSpeedFlatBonus;

        public virtual float getAttackSpeed()
        {
            return attackSpeedBase * attackSpeedMultiplier + attackSpeedFlatBonus;
        }
    #endregion

    #region Move Speed
        protected float moveSpeedBase;
        protected float moveSpeedMultiplier;
        protected float moveSpeedFlatBonus;

        public virtual float getMoveSpeed()
        {
            return moveSpeedBase * moveSpeedMultiplier + moveSpeedFlatBonus;
        }

        public void SetMoveSpeedBase(float value)
        {
            moveSpeedBase = value;
        }

        public float GetMoveSpeedBase()
        {
            return moveSpeedBase;
        }
    #endregion
    
    #region Defense
        protected float defenseBase;
        protected float defenseMultiplier;
        protected float defenseFlatBonus;

        public virtual float getDefense()
        {
            return defenseBase * defenseMultiplier + defenseFlatBonus;
        }
    #endregion
    
    #region Dodge Chance
        protected float dodgeChanceBase;
        protected float dodgeChanceFlatBonus;
        protected float dodgeChanceMultiplier;

        public virtual float getDodgeChance()
        {
            return dodgeChanceBase * dodgeChanceMultiplier + dodgeChanceFlatBonus;
        }
    #endregion

    #region Crit Chance
        protected float critChanceBase;
        protected float critChanceFlatBonus;
        protected float critChanceMultiplier;

        public virtual float getCritChance()
        {
            return critChanceBase * critChanceMultiplier + critChanceFlatBonus;
        }
    #endregion

    #region Crit Damage
        protected float critDamageBase;
        protected float critDamageMultiplier;
        protected float critDamageFlatBonus;

        public virtual float getCritDamage()
        {
            return critDamageBase * critDamageMultiplier + critDamageFlatBonus;
        }
    #endregion
    
    #region Trap Damage Resist
        protected float trapDamageResistBase;
        protected float trapDamageResistMultiplier;
        protected float trapDamageResistFlatBonus;

        public virtual float getTrapDamageResist()
        {
            return trapDamageResistBase * trapDamageResistMultiplier + trapDamageResistFlatBonus;
        }
    #endregion

}
