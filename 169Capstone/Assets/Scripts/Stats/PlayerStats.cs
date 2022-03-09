using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerFacingStatName
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
            CooldownReduction,
            ShopPriceReduction,
            Luck,
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
                case PlayerStatType.CooldownReduction:
                    if(bonusType == BonusType.flat)
                        cooldownReductionFlatBonus = total;
                    break;

                case PlayerStatType.ShopPriceReduction:
                    if(bonusType == BonusType.flat)
                        shopPriceReductionFlatBonus = total;
                    break;
                    
                case PlayerStatType.Luck:
                    if(bonusType == BonusType.flat)
                        luckFlatBonus = total;
                    else
                        luckMultiplier = total;
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
            const float defenseBonusPerStrengthPoint = 0.1f;

            public int Strength()
            {
                return strength;
            }
        #endregion

        #region Dexterity
            private int dexterity;

            const float dexterityDamagePerDexterityPoint = 1f;
            const float dodgeBonusPerDexterityPoint = 0.1f;

            public int Dexterity()
            {
                return dexterity;
            }
        #endregion

        #region Constitution
            private int constitution;

            const float maxHitPointBonusPerConstitutionPoint = 3f;
            const float statusResistBonusPerConstitutionPoint = 1f;

            public int Constitution()
            {
                return constitution;
            }
        #endregion

        #region Intelligence
            private int intelligence;

            const float intelligenceDamagePerIntelligencePoint = 1f;
            const float critChanceBonusPerIntelligencePoint = 0.5f;

            public int Intelligence()
            {
                return intelligence;
            }
        #endregion

        #region Wisdom
            private int wisdom;

            const float wisdomDamagePerWisdomPoint = 1f;
            const float cooldownReductionPerWisdomPoint = 1f;

            public int Wisdom()
            {
                return wisdom;
            }
        #endregion

        #region Charisma
            private int charisma;

            const float shopPriceReductionPerCharismaPoint = 0.5f;
            const float luckPerCharismaPoint = 0.1f;

            public int Charisma()
            {
                return charisma;
            }
        #endregion

    #endregion

    #region Player Specific Stats

        #region Cooldown Reduction
            private float cooldownReductionBase;
            private float cooldownReductionFlatBonus;

            public virtual float getCooldownReduction()
            {
                return cooldownReductionBase + cooldownReductionFlatBonus 
                        + (wisdom * cooldownReductionPerWisdomPoint);
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

        #region Luck
            private float luckBase;
            private float luckMultiplier;
            private float luckFlatBonus;

            public virtual float getLuck()
            {
                return luckBase * luckMultiplier + luckFlatBonus 
                        + (charisma * luckPerCharismaPoint);
            }
        #endregion

        #region Healing Efficacy
            private int healingEfficacyBase;
            private int healingEfficacyFlatBonus;

            public virtual int getHealingEfficacy()
            {
                return healingEfficacyBase + healingEfficacyFlatBonus;
            }
        #endregion

    #endregion

    #region Stat Calculators

        public override float getMaxHitPoints()
        {
            return maxHitPointsBase * maxHitPointsMultiplier + maxHitPointsFlatBonus
                    + (constitution * maxHitPointBonusPerConstitutionPoint);
        }

        public override float getAttackSpeed()
        {
            return base.getAttackSpeed(); //No differences from the base as of yet
        }

        public override float getMoveSpeed()
        {
            return base.getMoveSpeed(); //No differences from the base as of yet
        }

        public override float getDefense()
        {
            return defenseBase * defenseMultiplier + defenseFlatBonus
                    + (strength * defenseBonusPerStrengthPoint);
        }

        public override float getDodgeChance()
        {
            return dodgeChanceBase + dodgeChanceFlatBonus
                    + (dexterity * dodgeBonusPerDexterityPoint);
        }

        public override float getCritChance()
        {
            return critChanceBase + critChanceFlatBonus
                    + (intelligence * critChanceBonusPerIntelligencePoint);
        }

        public override float getCritDamage()
        {
            return base.getCritDamage(); //No differences from the base as of yet
        }

        public override float getStunChance()
        {
            return base.getStunChance(); //No differences from the base as of yet
        }

        public override float getBurnChance()
        {
            return base.getBurnChance(); //No differences from the base as of yet
        }

        public override float getSlowChance()
        {
            return base.getSlowChance(); //No differences from the base as of yet
        }

        public override float getStatusResistChance()
        {
            return statusResistChanceBase + statusResistChanceFlatBonus
                    + (constitution * statusResistBonusPerConstitutionPoint);
        }

    #endregion

    protected override void Awake()
    {
        base.Awake();        
    }

    void OnEnable()
    {
        initializeStats();
    }

    public void initializeStats()
    {
        PermanentUpgradeManager pum = PermanentUpgradeManager.instance;

        attackSpeedBase = pum.GetCurrentSkillValue(PermanentUpgradeType.ExtensiveTraining);
        defenseBase = pum.GetCurrentSkillValue(PermanentUpgradeType.ArmorPlating);
        critChanceBase = pum.GetCurrentSkillValue(PermanentUpgradeType.Natural20);
        critDamageBase = pum.GetCurrentSkillValue(PermanentUpgradeType.PrecisionDrive);
        
        moveSpeedBase = 1;

        maxHitPointsMultiplier = 1;
        attackSpeedMultiplier = 1;
        moveSpeedMultiplier = 1;
        defenseMultiplier = 1;
        luckMultiplier = 1;

        float relativeWeight = 2;

        //TODO implement skill point "pool"

        strength = randomOnCurve(pum.strMin, pum.strMax, relativeWeight);
        dexterity = randomOnCurve(pum.dexMin, pum.dexMax, relativeWeight);
        constitution = randomOnCurve(pum.conMin, pum.conMax, relativeWeight);
        intelligence = randomOnCurve(pum.intMin, pum.intMax, relativeWeight);
        wisdom = randomOnCurve(pum.wisMin, pum.wisMax, relativeWeight);
        charisma = randomOnCurve(pum.charismaMin, pum.charismaMax, relativeWeight);

        healingEfficacyBase = 25;
        healingEfficacyFlatBonus = 0;
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
    }

    public void SetHealingEfficacyFlatBonus(int value)
    {
        healingEfficacyFlatBonus = value;
    }
}
