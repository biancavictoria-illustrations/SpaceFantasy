using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatName
{
    STR,
    DEX,
    INT,
    WIS,
    CHA,
    CON,
    size
}

public class PlayerStats : EntityStats
{
    #region Bonus Management
        public enum PlayerStatType
        {
            ShopPriceReduction,
            HealingEfficacy,

            enumSize
        }

        private struct PlayerStatBonusType : System.IEquatable<PlayerStatBonusType>
        {
            public PlayerStatBonusType(PlayerStatType s, BonusType b)
            {
                stat = s;
                bonus = b;
            }

            public PlayerStatType stat;
            public BonusType bonus;

            public override bool Equals(object obj) => obj is PlayerStatBonusType other && this.Equals(other);

            public bool Equals(PlayerStatBonusType other)
            {
                return stat == other.stat && bonus == other.bonus;
            }

            public override int GetHashCode() => ((int)stat, (int)bonus).GetHashCode();
        }

        private Dictionary<Object, Dictionary<PlayerStatBonusType, float>> playerStatBonusFromSource;

        public void SetBonusForStat(Object bonusSource, PlayerStatType stat, BonusType bonusType, float bonusAmount)
        {
            if(bonusSource == null || stat == PlayerStatType.enumSize ||  bonusType == BonusType.enumSize)
                return;
            
            if(!playerStatBonusFromSource.ContainsKey(bonusSource))
            {
                playerStatBonusFromSource.Add(bonusSource, new Dictionary<PlayerStatBonusType, float>());
            }

            PlayerStatBonusType sbt = new PlayerStatBonusType(stat, bonusType);
            if(playerStatBonusFromSource[bonusSource].ContainsKey(sbt))
                playerStatBonusFromSource[bonusSource][sbt] = bonusAmount;
            else
                playerStatBonusFromSource[bonusSource].Add(sbt, bonusAmount);

            RecalculateStatBonus(stat, bonusType);
        }

        public float? GetBonusForStat(Object bonusSource, PlayerStatType stat, BonusType bonusType)
        {
            if(bonusSource == null || stat == PlayerStatType.enumSize ||  bonusType == BonusType.enumSize)
                return null;
            
            if(!playerStatBonusFromSource.ContainsKey(bonusSource))
                return null;

            PlayerStatBonusType sbt = new PlayerStatBonusType(stat, bonusType);
            if(playerStatBonusFromSource[bonusSource].ContainsKey(sbt))
                return playerStatBonusFromSource[bonusSource][sbt];
            else
                return null;
        }

        private void RecalculateStatBonus(PlayerStatType stat, BonusType bonusType)
        {
            if(stat == PlayerStatType.enumSize ||  bonusType == BonusType.enumSize)
                return;

            float total = 0;
            PlayerStatBonusType sbt = new PlayerStatBonusType(stat, bonusType);
            foreach(Object key in new List<Object>(playerStatBonusFromSource.Keys))
            {
                if(key == null)
                {
                    playerStatBonusFromSource.Remove(key);
                }
                else
                {
                    if(playerStatBonusFromSource[key].ContainsKey(sbt))
                        total += playerStatBonusFromSource[key][sbt];
                }
            }

            switch(stat)
            {
                case PlayerStatType.ShopPriceReduction:
                    if(bonusType == BonusType.flat)
                        shopPriceReductionFlatBonus = total;
                    break;
                    
                case PlayerStatType.HealingEfficacy:
                    if(bonusType == BonusType.flat)
                        healingEfficacyFlatBonus = (int)total;
                    break;
            }
        }
    #endregion

    #region Primary Stats

        #region Strength
            private int strength;

            const float strengthDamagePerStrengthPoint = 1f;
            const float defenseBonusPerStrengthPoint = 0.005f;

            public int Strength()
            {
                return strength;
            }
        #endregion

        #region Dexterity
            private int dexterity;

            const float dexterityDamagePerDexterityPoint = 1f;
            const float dodgeBonusPerDexterityPoint = 0.005f;

            public int Dexterity()
            {
                return dexterity;
            }
        #endregion

        #region Constitution
            private int constitution;

            const float maxHitPointBonusPerConstitutionPoint = 3f;
            const float trapDamageResistBonusPerConstitutionPoint = 0.01f;

            public int Constitution()
            {
                return constitution;
            }
        #endregion

        #region Intelligence
            private int intelligence;

            const float intelligenceDamagePerIntelligencePoint = 1f;
            const float critChanceBonusPerIntelligencePoint = 0.01f;

            public int Intelligence()
            {
                return intelligence;
            }
        #endregion

        #region Wisdom
            private int wisdom;

            const float wisdomDamagePerWisdomPoint = 1f;
            const float hastePerWisdomPoint = 0.01f;

            public int Wisdom()
            {
                return wisdom;
            }
        #endregion

        #region Charisma
            private int charisma;

            const float shopPriceReductionPerCharismaPoint = 0.01f;

            public int Charisma()
            {
                return charisma;
            }
        #endregion

    #endregion

    #region Player Specific Stats

        #region Haste
            // Haste = Cooldown Reduction
            public virtual float getHaste()
            {
                return hasteBase + hasteFlatBonus 
                        + (wisdom * hastePerWisdomPoint);
            }
        #endregion

        #region Shop Price Reduction
            private float shopPriceReductionBase;
            private float shopPriceReductionFlatBonus;

            public virtual float getShopPriceReduction()
            {
                return shopPriceReductionBase + shopPriceReductionFlatBonus 
                        + (charisma * shopPriceReductionPerCharismaPoint);
            }
        #endregion

        #region Healing Efficacy
            private float healingEfficacyBase;
            private float healingEfficacyFlatBonus;

            public virtual float getHealingEfficacy()
            {
                return healingEfficacyBase + healingEfficacyFlatBonus;
            }
        #endregion

    #endregion

    #region Stat Calculators

        public override float getMaxHitPoints()
        {
            return (constitution * maxHitPointBonusPerConstitutionPoint) * maxHitPointsMultiplier + maxHitPointsFlatBonus;
        }

        public override float getAttackSpeed()
        {
            return base.getAttackSpeed(); //No differences from the base as of yet
        }

        public override float getMoveSpeed()
        {
            return (moveSpeedBase + moveSpeedFlatBonus) * moveSpeedMultiplier;
        }

        public override float getDefense()
        {
            return (defenseBase + defenseFlatBonus + (strength * defenseBonusPerStrengthPoint)) * defenseMultiplier;
        }

        public override float getDodgeChance()
        {
            return (dodgeChanceBase + dodgeChanceFlatBonus + (dexterity * dodgeBonusPerDexterityPoint)) * dodgeChanceMultiplier;
        }

        public override float getCritChance()
        {
            return (critChanceBase + critChanceFlatBonus + (intelligence * critChanceBonusPerIntelligencePoint)) * critChanceMultiplier;
        }

        public override float getCritDamage()
        {
            return base.getCritDamage(); //No differences from the base as of yet
        }

        public override float getTrapDamageResist()
        {
            return trapDamageResistBase + trapDamageResistFlatBonus
                    + (constitution * trapDamageResistBonusPerConstitutionPoint);
        }

    #endregion

    #region Damage Values
        public virtual float getSTRDamage(bool enableCrit = true)
        {
            float damage = strength * STRDamageMultiplier + STRDamageFlatBonus;
            if(enableCrit){
                damage += CalculateCritValue(damage);
            }
            return damage;
        }

        public virtual float getDEXDamage(bool enableCrit = true)
        {
            float damage = dexterity * DEXDamageMultiplier + DEXDamageFlatBonus;
            if(enableCrit){
                damage += CalculateCritValue(damage);
            }
            return damage;
        }

        public virtual float getWISDamage(bool enableCrit = true)
        {
            float damage = wisdom * WISDamageMultiplier + WISDamageFlatBonus;
            if(enableCrit){
                damage += CalculateCritValue(damage);
            }
            return damage;
        }

        public virtual float getINTDamage(bool enableCrit = true)
        {
            float damage = intelligence * INTDamageMultiplier + INTDamageFlatBonus;
            if(enableCrit){
                damage += CalculateCritValue(damage);
            }
            return damage;
        }

        private float CalculateCritValue(float baseDamage)
        {
            //EntityHealth healthScript = GetComponent<EntityHealth>();
            float chance = Random.Range(0.0f, 1f);
            if(chance <= getCritChance()){
                float crit = getCritDamage();
                //healthScript.OnCrit.Invoke(healthScript, crit);
                return baseDamage * crit;
            }
            return 0f;
        }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        initializeStats();
    }

    public void initializeStats()
    {
        PermanentUpgradeManager pum = PermanentUpgradeManager.instance;

        attackSpeedBase = 1 + pum.GetCurrentSkillValue(PermanentUpgradeType.ExtensiveTraining);
        defenseBase = pum.GetCurrentSkillValue(PermanentUpgradeType.ArmorPlating);
        critChanceBase = pum.GetCurrentSkillValue(PermanentUpgradeType.Natural20);
        critDamageBase = 1 + pum.GetCurrentSkillValue(PermanentUpgradeType.PrecisionDrive);
        
        moveSpeedBase = 1;
        hasteBase = 1;

        critChanceMultiplier = 1;
        critDamageMultiplier = 1;
        maxHitPointsMultiplier = 1;
        attackSpeedMultiplier = 1;
        moveSpeedMultiplier = 1;
        defenseMultiplier = 1;

        STRDamageMultiplier = 1;
        DEXDamageMultiplier = 1;
        WISDamageMultiplier = 1;
        INTDamageMultiplier = 1;

        // float relativeWeight = 2;

        int numSkillPoints = Mathf.CeilToInt((pum.strMin + pum.strMax + pum.dexMin + pum.dexMax + pum.conMin + pum.conMax + pum.intMin + pum.intMax + pum.wisMin + pum.wisMax + pum.charismaMin + pum.charismaMax) / 2f);
        
        if( GameManager.instance.currentRunNumber == 1 ){
            SetFirstRunStatValues();
        }
        else{
            strength = generateStat(PlayerStatName.STR, pum, pum.strMin, pum.strMax, numSkillPoints);
            numSkillPoints -= strength;

            dexterity = generateStat(PlayerStatName.DEX, pum, pum.dexMin, pum.dexMax, numSkillPoints);
            numSkillPoints -= dexterity;

            constitution = generateStat(PlayerStatName.CON, pum, pum.conMin, pum.conMax, numSkillPoints);
            numSkillPoints -= constitution;

            intelligence = generateStat(PlayerStatName.INT, pum, pum.intMin, pum.intMax, numSkillPoints);
            numSkillPoints -= intelligence;

            wisdom = generateStat(PlayerStatName.WIS, pum, pum.wisMin, pum.wisMax, numSkillPoints);
            numSkillPoints -= wisdom;

            charisma = numSkillPoints;
        }

        healingEfficacyBase = 0.25f;
        healingEfficacyFlatBonus = 0;
    }

    private void SetFirstRunStatValues()
    {
        strength = 10;
        dexterity = 10;
        constitution = 10;
        intelligence = 10;
        wisdom = 10;
        charisma = 10;
    }

    private int generateStat(PlayerStatName stat, PermanentUpgradeManager pum, int minValue, int maxValue, int numSkillPoints, float relativeWeight = 2)
    {
        int statValue = randomOnCurve(minValue, maxValue, relativeWeight);

        int maxThreshold = getMaxPointThresholdForRemainingStats(pum, stat);
        if(numSkillPoints - statValue > maxThreshold)
        {
            statValue = numSkillPoints - maxThreshold;
        }

        int minThreshold = getMinPointThresholdForRemainingStats(pum, stat);
        if(numSkillPoints - statValue < minThreshold)
        {
            statValue = numSkillPoints - minThreshold;
        }

        return statValue;
    }

    private int getMinPointThresholdForRemainingStats(PermanentUpgradeManager pum, PlayerStatName stat)
    {
        int num = 0;
        switch(stat)
        {
            case PlayerStatName.STR:
                num += pum.dexMin;
                goto case PlayerStatName.DEX;

            case PlayerStatName.DEX:
                num += pum.conMin;
                goto case PlayerStatName.CON;

            case PlayerStatName.CON:
                num += pum.intMin;
                goto case PlayerStatName.INT;

            case PlayerStatName.INT:
                num += pum.wisMin;
                goto case PlayerStatName.WIS;

            case PlayerStatName.WIS:
                num += pum.charismaMin;
                break;
        }
        return num;
    }

    private int getMaxPointThresholdForRemainingStats(PermanentUpgradeManager pum, PlayerStatName stat)
    {
        int num = 0;
        switch(stat)
        {
            case PlayerStatName.STR:
                num += pum.dexMax;
                goto case PlayerStatName.DEX;

            case PlayerStatName.DEX:
                num += pum.conMax;
                goto case PlayerStatName.CON;

            case PlayerStatName.CON:
                num += pum.intMax;
                goto case PlayerStatName.INT;

            case PlayerStatName.INT:
                num += pum.wisMax;
                goto case PlayerStatName.WIS;

            case PlayerStatName.WIS:
                num += pum.charismaMax;
                break;
        }
        return num;
    }

    private int randomOnCurve(int min, int max, float relativeWeight)
    {
        float a = ( 4*(relativeWeight - 1) )/(-Mathf.Pow(min, 2) + 2 * min * max - Mathf.Pow(max, 2));

        float formula(float x)
        {
            return a * (x - min) * (x - max) + 1;
        }

        Dictionary<int, float> statWeights = new Dictionary<int, float>();
        float totalWeight = 0;

        for(int i = min; i <= max; ++i)
        {
            totalWeight += formula(i);
            statWeights.Add(i, totalWeight);
        }

        int value = min;
        float weight = Random.Range(0, totalWeight);

        while(value < max && weight > statWeights[value])
        {
            ++value;
        }

        return value;
    }

    public void SetStrength(int value)
    {
        strength = value;
    }

    public void SetDexterity(int value)
    {
        dexterity = value;
    }

    public void SetIntelligence(int value)
    {
        intelligence = value;
    }

    public void SetWisdom(int value)
    {
        wisdom = value;
    }

    public void SetCharisma(int value)
    {
        charisma = value;
    }

    public void SetConstitution(int value)
    {
        constitution = value;
        Player.instance.health.UpdateHealthOnUpgrade();
    }

    public void SetHealingEfficacyFlatBonus(float value)
    {
        healingEfficacyFlatBonus = value;
    }

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
            case StatType.TrapDamageResist:
                return getTrapDamageResist();
            case StatType.DodgeChance:
                return getDodgeChance();
            case StatType.HitPoints:
                return getMaxHitPoints();

            // Primary Lines ONLY
            case StatType.STRDamage:
                return getSTRDamage(false);
            case StatType.DEXDamage:
                return getDEXDamage(false);
            case StatType.INTDamage:
                return getINTDamage(false);
            case StatType.WISDamage:
                return getWISDamage(false);    
        }
        Debug.LogError("No current value found for stat type: " + type);
        return -1;
    }
}
