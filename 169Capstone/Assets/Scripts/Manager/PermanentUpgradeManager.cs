using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentUpgradeManager : MonoBehaviour
{
    public static PermanentUpgradeManager instance;

    [HideInInspector] public int totalPermanentCurrencySpent = 0;

    public int startingHealthPotionQuantity = 3;    // Putting this here in case it ends up being something you can upgrade (should prob start @ 0)

    // Skills
    public int levelsInArmorPlating {get; private set;}
    public const int maxArmorPlatingLevels = 5;
    public readonly float armorPlatingBonusPerLevel = 2f;
    
    public int levelsInExtensiveTraining {get; private set;}
    public const int maxExtensiveTrainingLevels = 5;
    public readonly float extensiveTrainingBonusPerLevel = 0.02f;
    
    public int levelsInNatural20 {get; private set;}
    public const int maxNatural20Levels = 1;
    public readonly float natural20BonusPerLevel = 0.05f;
    
    public int levelsInPrecisionDrive {get; private set;}
    public const int maxPrecisionDriveLevels = 3;
    public readonly float[] precisionDriveBonusPerLevel = new float[] {0f, 0.1f, 0.25f, 0.5f};

    public int levelsInTimeLichKillerThing {get; private set;}
    public const int maxTimeLichThingLevels = 1;

    // Stats
    public const int DEFAULT_MIN = 5;
    public const int DEFAULT_MAX = 15;

    public int strMin {get; private set;}
    public int strMax {get; private set;}

    public int dexMin {get; private set;}
    public int dexMax {get; private set;}

    public int intMin {get; private set;}
    public int intMax {get; private set;}

    public int wisMin {get; private set;}
    public int wisMax {get; private set;}

    public int conMin {get; private set;}
    public int conMax {get; private set;}

    public int charismaMin {get; private set;}
    public int charismaMax {get; private set;}
    

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        SetupPermanentUpgradeValues();
    }
    
    public void SetupPermanentUpgradeValues()
    {
        // If you have saved stats, set them; if not, set to default
        if(false){
            
        }
        else{
            ResetAllStatGenerationValuesToDefault();
            ResetAllSkillValuesToDefault();
        }
    }

    public void ResetAllSkillValuesToDefault()
    {
        levelsInArmorPlating = 0;
        levelsInExtensiveTraining = 0;
        levelsInNatural20 = 0;
        levelsInPrecisionDrive = 0;
        levelsInTimeLichKillerThing = 0;
    }

    public void SetSkillLevel(PermanentUpgradeType upgradeType, int level)
    {
        switch(upgradeType){
            case PermanentUpgradeType.ArmorPlating:
                levelsInArmorPlating = level;
                return;
            case PermanentUpgradeType.ExtensiveTraining:
                levelsInExtensiveTraining = level;
                return;
            case PermanentUpgradeType.Natural20:
                levelsInNatural20 = level;
                return;
            case PermanentUpgradeType.PrecisionDrive:
                levelsInPrecisionDrive = level;
                return;
            case PermanentUpgradeType.TimeLichKillerThing:
                levelsInTimeLichKillerThing = level;
                return;
        }
        Debug.LogWarning("No data found for upgrade type: " + upgradeType);
    }

    public int GetSkillLevel(PermanentUpgradeType upgradeType)
    {
        switch(upgradeType){
            case PermanentUpgradeType.ArmorPlating:
                return levelsInArmorPlating;
            case PermanentUpgradeType.ExtensiveTraining:
                return levelsInExtensiveTraining;
            case PermanentUpgradeType.Natural20:
                return levelsInNatural20;
            case PermanentUpgradeType.PrecisionDrive:
                return levelsInPrecisionDrive;
            case PermanentUpgradeType.TimeLichKillerThing:
                return levelsInTimeLichKillerThing;
        }
        Debug.LogWarning("No data found for upgrade type: " + upgradeType);
        return -1;
    }

    public float GetCurrentSkillValue(PermanentUpgradeType upgradeType)
    {
        switch(upgradeType){
            case PermanentUpgradeType.ArmorPlating:
                return levelsInArmorPlating * armorPlatingBonusPerLevel;
            case PermanentUpgradeType.ExtensiveTraining:
                return 1 + levelsInExtensiveTraining * extensiveTrainingBonusPerLevel;
            case PermanentUpgradeType.Natural20:
                return levelsInNatural20 * natural20BonusPerLevel;
            case PermanentUpgradeType.PrecisionDrive:
                return precisionDriveBonusPerLevel[levelsInPrecisionDrive];
            case PermanentUpgradeType.TimeLichKillerThing:
                return levelsInTimeLichKillerThing;
        }
        Debug.LogWarning("No data found for upgrade type: " + upgradeType);
        return -1;
    }

    public void ResetAllStatGenerationValuesToDefault()
    {
        strMin = DEFAULT_MIN;
        dexMin = DEFAULT_MIN;
        intMin = DEFAULT_MIN;
        wisMin = DEFAULT_MIN;
        conMin = DEFAULT_MIN;
        charismaMin = DEFAULT_MIN;

        strMax = DEFAULT_MAX;
        dexMax = DEFAULT_MAX;
        intMax = DEFAULT_MAX;
        wisMax = DEFAULT_MAX;
        conMax = DEFAULT_MAX;
        charismaMax = DEFAULT_MAX;
    }

    public void SetStrengthMin(int value)
    {
        if(value > strMax){
            Debug.LogWarning("Unable to set min stat above max.");
            return;
        }
        strMin = value;
    }

    public void SetStrengthMax(int value)
    {
        if(value < strMin){
            Debug.LogWarning("Unable to set max stat below min.");
            return;
        }
        strMax = value;
    }

    public void SetDexterityMin(int value)
    {
        if(value > dexMax){
            Debug.LogWarning("Unable to set min stat above max.");
            return;
        }

        dexMin = value;
    }

    public void SetDexterityMax(int value)
    {
        if(value < dexMin){
            Debug.LogWarning("Unable to set max stat below min.");
            return;
        }
        dexMax = value;
    }

    public void SetIntMin(int value)
    {
        if(value > intMax){
            Debug.LogWarning("Unable to set min stat above max.");
            return;
        }
        intMin = value;
    }

    public void SetIntMax(int value)
    {
        if(value < intMin){
            Debug.LogWarning("Unable to set max stat below min.");
            return;
        }
        intMax = value;
    }

    public void SetWisdomMin(int value)
    {
        if(value > wisMax){
            Debug.LogWarning("Unable to set min stat above max.");
            return;
        }
        wisMin = value;
    }

    public void SetWisdomMax(int value)
    {
        if(value < wisMin){
            Debug.LogWarning("Unable to set max stat below min.");
            return;
        }
        wisMax = value;
    }

    public void SetConMin(int value)
    {
        if(value > conMax){
            Debug.LogWarning("Unable to set min stat above max.");
            return;
        }
        conMin = value;
    }

    public void SetConMax(int value)
    {
        if(value < conMin){
            Debug.LogWarning("Unable to set max stat below min.");
            return;
        }
        conMax = value;
    }

    public void SetCharismaMin(int value)
    {
        if(value > charismaMax){
            Debug.LogWarning("Unable to set min stat above max.");
            return;
        }
        charismaMin = value;
    }

    public void SetCharismaMax(int value)
    {
        if(value < charismaMin){
            Debug.LogWarning("Unable to set max stat below min.");
            return;
        }
        charismaMax = value;
    }

    public int GetStatGenerationValueFromUpgradeType(PermanentUpgradeType upgradeType)
    {
        if((int)upgradeType > 11){
            Debug.LogWarning("Cannot get stat generation value for upgrade type: " + upgradeType);
            return -1;
        }

        switch(upgradeType){
            case PermanentUpgradeType.STRMin:
                return strMin;
            case PermanentUpgradeType.STRMax:
                return strMax;
            case PermanentUpgradeType.DEXMin:
                return dexMin;
            case PermanentUpgradeType.DEXMax:
                return dexMax;
            case PermanentUpgradeType.INTMin:
                return intMin;
            case PermanentUpgradeType.INTMax:
                return intMax;
            case PermanentUpgradeType.WISMin:
                return wisMin;
            case PermanentUpgradeType.WISMax:
                return wisMax;
            case PermanentUpgradeType.CONMin:
                return conMin;
            case PermanentUpgradeType.CONMax:
                return conMax;
            case PermanentUpgradeType.CHAMin:
                return charismaMin;
            case PermanentUpgradeType.CHAMax:
                return charismaMax;
        }

        Debug.LogError("Could not find stat generation value for upgrade type: " + upgradeType);
        return -1;
    }
}