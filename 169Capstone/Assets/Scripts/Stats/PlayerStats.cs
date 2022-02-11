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

    #region Primary Stats

        #region Strength
            private int strength;
            private int minStrength = 5;
            private int maxStrength = 15;

            const float strengthDamagePerStrengthPoint = 1f;
            const float defenseBonusPerStrengthPoint = 0.1f;

            public int Strength()
            {
                return strength;
            }
        #endregion

        #region Dexterity
            private int dexterity;
            private int minDexterity = 5;
            private int maxDexterity = 15;

            const float dexterityDamagePerDexterityPoint = 1f;
            const float dodgeBonusPerDexterityPoint = 0.1f;

            public int Dexterity()
            {
                return dexterity;
            }
        #endregion

        #region Constitution
            private int constitution;
            private int minConstitution = 5;
            private int maxConstitution = 15;

            const float maxHitPointBonusPerConstitutionPoint = 3f;
            const float statusResistBonusPerConstitutionPoint = 1f;

            public int Constitution()
            {
                return constitution;
            }
        #endregion

        #region Intelligence
            private int intelligence;
            private int minIntelligence = 5;
            private int maxIntelligence = 15;

            const float intelligenceDamagePerIntelligencePoint = 1f;
            const float critChanceBonusPerIntelligencePoint = 0.5f;

            public int Intelligence()
            {
                return intelligence;
            }
        #endregion

        #region Wisdom
            private int wisdom;
            private int minWisdom = 5;
            private int maxWisdom = 15;

            const float wisdomDamagePerWisdomPoint = 1f;
            const float cooldownReductionPerWisdomPoint = 1f;

            public int Wisdom()
            {
                return wisdom;
            }
        #endregion

        #region Charisma
            private int charisma;
            private int minCharisma = 5;
            private int maxCharisma = 15;

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
            return statusResistChanceBase * statusResistChanceMultiplier + statusResistChanceFlatBonus
                    + (constitution * statusResistBonusPerConstitutionPoint);
        }

    #endregion

    void Awake()
    {
        initializeStats();
    }

    public void initializeStats()
    {
        float relativeWeight = 2;

        //TODO implement skill point "pool"

        strength = randomOnCurve(minStrength, maxStrength, relativeWeight);
        dexterity = randomOnCurve(minDexterity, maxDexterity, relativeWeight);
        constitution = randomOnCurve(minConstitution, maxConstitution, relativeWeight);
        intelligence = randomOnCurve(minIntelligence, maxIntelligence, relativeWeight);
        wisdom = randomOnCurve(minWisdom, maxWisdom, relativeWeight);
        charisma = randomOnCurve(minCharisma, maxCharisma, relativeWeight);
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
