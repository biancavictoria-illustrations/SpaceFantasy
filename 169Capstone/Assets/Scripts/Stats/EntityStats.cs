using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    HitPoints,
    AttackSpeed,
    MoveSpeed,
    Defense,
    DodgeChance,
    CritChance,
    CritDamage,
    StunChance,
    BurnChance,
    SlowChance,
    StatusResist,

    enumSize
}

public abstract class EntityStats : MonoBehaviour
{
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
            
            if(!statBonusFromSource.ContainsKey(bonusSource))
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

                case StatType.StunChance:
                    if(bonusType == BonusType.flat)
                        stunChanceFlatBonus = total;
                    break;

                case StatType.BurnChance:
                    if(bonusType == BonusType.flat)
                        burnChanceFlatBonus = total;
                    break;

                case StatType.SlowChance:
                    if(bonusType == BonusType.flat)
                        slowChanceFlatBonus = total;
                    break;

                case StatType.StatusResist:
                    if(bonusType == BonusType.flat)
                        statusResistChanceFlatBonus = total;
                    break;
            }
        }
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

        public virtual float getDodgeChance()
        {
            return dodgeChanceBase + dodgeChanceFlatBonus;
        }
    #endregion

    #region Crit Chance
        protected float critChanceBase;
        protected float critChanceFlatBonus;

        public virtual float getCritChance()
        {
            return critChanceBase + critChanceFlatBonus;
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

    #region Stun Chance
        protected float stunChanceBase;
        protected float stunChanceFlatBonus;

        public virtual float getStunChance()
        {
            return stunChanceBase + stunChanceFlatBonus;
        }
    #endregion

    #region Burn Chance
        protected float burnChanceBase;
        protected float burnChanceFlatBonus;

        public virtual float getBurnChance()
        {
            return burnChanceBase + burnChanceFlatBonus;
        }
    #endregion
    
    #region Slow Chance
        protected float slowChanceBase;
        protected float slowChanceFlatBonus;

        public virtual float getSlowChance()
        {
            return slowChanceBase + slowChanceFlatBonus;
        }
    #endregion
    
    #region Status Resist Chance
        protected float statusResistChanceBase;
        protected float statusResistChanceFlatBonus;

        public virtual float getStatusResistChance()
        {
            return statusResistChanceBase + statusResistChanceFlatBonus;
        }
    #endregion

    public float GetCurrentValueFromStatType(StatType type)
    {
        switch(type){
            case StatType.CritChance:
                return getCritChance();
            case StatType.CritDamage:
                return getCritDamage();
            case StatType.AttackSpeed:
                return getAttackSpeed();
            case StatType.Defense:
                return getDefense();
            case StatType.MoveSpeed:
                return getMoveSpeed();
            case StatType.StunChance:
                return getStunChance();
            case StatType.BurnChance:
                return getBurnChance();
            case StatType.SlowChance:
                return getSlowChance();
            case StatType.StatusResist:
                return getStatusResistChance();
            case StatType.DodgeChance:
                return getDodgeChance();
            case StatType.HitPoints:
                return getMaxHitPoints();
        }
        Debug.LogError("No current value found for stat type: " + type);
        return -1;
    }

    public float GetBaseValueFromStatType(StatType type)
    {
        switch(type){
            case StatType.CritChance:
                return critChanceBase;
            case StatType.CritDamage:
                return critDamageBase;
            case StatType.AttackSpeed:
                return attackSpeedBase;
            case StatType.Defense:
                return defenseBase;
            case StatType.MoveSpeed:
                return moveSpeedBase;
            case StatType.StunChance:
                return stunChanceBase;
            case StatType.BurnChance:
                return burnChanceBase;
            case StatType.SlowChance:
                return slowChanceBase;
            case StatType.StatusResist:
                return statusResistChanceBase;
            case StatType.DodgeChance:
                return dodgeChanceBase;
            case StatType.HitPoints:
                return maxHitPointsBase;
        }
        Debug.LogError("No base value found for stat type: " + type);
        return -1;
    }
}
