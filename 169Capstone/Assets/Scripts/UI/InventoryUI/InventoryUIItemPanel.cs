using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class InventoryUIItemPanel : MonoBehaviour
{
    // Values that descriptions can include as variables based on stats
    private enum DescriptionVariableCode{
        // Primary Stats
        STR,
        DEX,
        INT,
        WIS,
        CHA,
        CON,

        // Damage Values
        SD,     // STR Damage
        DD,     // DEX Damage
        WD,     // WIS Damage
        ID,     // INT Damage

        // Secondary Stats
        ATS,    // Attack Speed
        MOS,    // Move Speed
        DEF,    // Defense
        DOC,    // Dodge Chance
        CRC,    // Crit Chance
        CRD,    // Crit Damage
        TDR,    // Trap Damage Resist
        HAS,    // Haste (Cooldown Reduction)

        enumSize
    }

    public const string STAT_VARIABLE_PATTERN = @"({(STR|DEX|INT|WIS|CHA|CON|SD|DD|WD|ID|ATS|MOS|DEF|DOC|CRC|CRD|TDR|HAS)(:\d+)?(,(\*|\+|-)(\d?.)?\d+(,(\*|\+|-)(\d?.)?\d+)?)?})";

    [SerializeField] private InventoryItemSlot itemSlot;
    [HideInInspector] public ItemRarity rarity;

    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemTypeRarity;
    public TMP_Text itemDescription;

    private string shortDescription = "";    // 1-2 lines
    private string expandedDescription = ""; // Detailed additions

    public GameObject descriptionPanel;
    public FlexibleGridLayout textGrid;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    private GeneratedEquipment itemData;

    [SerializeField] private Toggle toggle;

    public void SetItemPanelValues(GeneratedEquipment _itemData)
    {
        itemData = _itemData;

        EquipmentBaseData baseData = itemData.equipmentBaseData;

        itemName.text = baseData.ItemName();
        rarity = itemData.rarity;

        itemTypeRarity.text = rarity.ToString() + " " + baseData.ItemSlot().ToString();

        shortDescription = baseData.ShortDescription();
        expandedDescription = GenerateExpandedDescription();
        itemDescription.text = shortDescription;

        itemIcon.sprite = baseData.Icon();
        itemIcon.preserveAspect = true;
        
        // Check bc compare item panel doesn't have a toggle
        if(toggle){
            toggle.interactable = true;
        }
    }

    public void SetDefaultItemPanelValues()
    {
        itemName.text = "";
        rarity = ItemRarity.enumSize;

        itemTypeRarity.text = itemSlot.ToString();

        shortDescription = "EMPTY";
        expandedDescription = "";
        itemDescription.text = shortDescription;

        itemIcon.sprite = InGameUIManager.instance.GetDefaultItemIconForSlot(itemSlot);
        itemIcon.preserveAspect = true;

        if(toggle){
            toggle.interactable = false;
        }
    }

    public InventoryItemSlot GetItemSlot()
    {
        return itemSlot;
    }

    public void SetExpandedDescription(bool set)
    {
        if(set){
            itemDescription.text = expandedDescription;
        }
        else{
            itemDescription.text = shortDescription;
        }
    }

    private string GenerateExpandedDescription()
    {
        string generatedDescription = itemData.equipmentBaseData.LongDescription();

        Regex rgx = new Regex(STAT_VARIABLE_PATTERN);
        foreach(Match match in rgx.Matches(generatedDescription)){
            // Get the value and location in the string
            string matchString = match.Value;
            int matchIndex = match.Index;

            // Generate the value
            string newStringValue = GetStatVariableValue(matchString);

            // Swap out that part of the string for the value
            generatedDescription = generatedDescription.Replace(matchString, newStringValue);
        }
        generatedDescription += GetStatModifierDescription();

        return generatedDescription;
    }

    // TODO: Truncate the decimals if they're too long
    private string GetStatModifierDescription()
    {
        string s = "\n";

        // Add primary line text
        StatType primaryLine = itemData.equipmentBaseData.PrimaryItemLine();
        float primaryLineValue = itemData.primaryLineValue;
        if(primaryLineValue > 0){
            s += SetStringForStatValue(primaryLine, primaryLineValue) + "\n";
        }

        // Add secondary lines
        s += "<size=70%>";
        for(int i = 0; i < EntityStats.numberOfSecondaryLineOptions; i++){
            float bonusValue = itemData.GetSecondaryLineValueFromStatType((StatType)i);

            bool percent = true;
            if((StatType)i == StatType.HitPoints || (StatType)i == StatType.Defense){
                percent = false;
            }

            if(bonusValue != 0){
                s += SetStringForStatValue((StatType)i, bonusValue, percent);
            }
        }

        return s;
    }

    private string SetStringForStatValue(StatType type, float bonusValue, bool percent=true)
    {
        string returnString = "\n<b>" + itemData.GetPlayerFacingStatName(type) + ":</b> " + GetColorModForStatValue(bonusValue, type) + "+";

        // If %
        if(percent){
            returnString += (bonusValue*100) + "%";
        }
        // If flat
        else{
            returnString += bonusValue;
        }

        return returnString + "</color>";
    }

    // Returns green if increase in value, red if decrease
    private string GetColorModForStatValue(float newBonusValue, StatType type)
    {
        string colorMod = "<color=";

        // If you currently have something in that slot, compare new value to that value
        if(PlayerInventory.instance.ItemSlotIsFull(itemSlot)){
            GeneratedEquipment currentlyEquippedItem = PlayerInventory.instance.gear[itemSlot].data;
            float? currentValue = Player.instance.stats.GetBonusForStat( currentlyEquippedItem, type, EntityStats.BonusType.flat);

            // If it gives a bonus for that stat and that bonus is higher than the new item's bonus
            if(currentValue != null && newBonusValue < currentValue){
                colorMod += InGameUIManager.magentaColor;
            }
            else{
                colorMod += InGameUIManager.slimeGreenColor;
            }
        }
        else{
            colorMod += InGameUIManager.slimeGreenColor;
        }

        return colorMod + ">";
    }

    private string GetStatVariableValue(string matchString)
    {
        DescriptionVariableCode code = GetCodeFromString(matchString);
        if(code == DescriptionVariableCode.enumSize){
            return "enumSize ERROR";
        }
        
        if(matchString.Contains("+-") || matchString.Contains("-+") || matchString.Contains("++") || matchString.Contains("--") || matchString.Contains("**")){
            Debug.LogError("Variable string cannot contain consecutive operators of the same type. Item Name: " + itemName.text);
            return "ERROR";
        }
        if(matchString.Contains("-*") || matchString.Contains("+*")){
            Debug.LogError("Variable string cannot contain additive operator followed by a *. Item Name: " + itemName.text);
            return "ERROR";
        }
        if(matchString.Contains("*+")){
            Debug.LogError("Variable string cannot contain * followed by \"+\". Item Name: " + itemName.text);
            return "ERROR";
        }

        if((matchString.Contains("+") && matchString.IndexOf("+", matchString.IndexOf("+")) > 0) || (matchString.Contains("-") && !matchString.Contains("*-") && matchString.IndexOf("-", matchString.IndexOf("-")) > 0)){
            Debug.LogError("Invalid description variable: Two matching operators found. Ignoring second operator. Item Name: " + itemName.text);
        }
        if(matchString.Contains("+") && matchString.Contains("-") && !matchString.Contains("*-") && (matchString.IndexOf("+", matchString.IndexOf("-")) > 0 || matchString.IndexOf("-", matchString.IndexOf("+")) > 0)){
            Debug.LogError("Invalid description variable: Cannot use both add and subtract. Ignoring second operator. Item Name: " + itemName.text);
        }

        // === Set numeric values ===
        // Get the percent value (after the ":")
        float percent = 100;  // Set default percent value to 100% (if no # specified, assume the entire stat value)
        int colonIndex = matchString.IndexOf(":");
        int commaIndex = matchString.IndexOf(",");  // Get the index of the first comma, if there is one
        if(colonIndex > -1){
            percent = GetNumberFromMatchStringGivenIndices(colonIndex, commaIndex, matchString);
        }

        // Get the * value
        float multiplier = 1;  // Set default value to 1 (if no # specified, assume nothing is being multiplied)
        int starIndex = matchString.IndexOf("*");
        commaIndex = matchString.IndexOf(",",commaIndex+1);  // Get the index of the next comma, if there is one
        if(starIndex > -1){
            multiplier = GetNumberFromMatchStringGivenIndices(starIndex, commaIndex, matchString);
        }

        // Get the + value
        float flatAddition = 0;  // Set default value to 0 (if no # specified, assume nothing is being added)
        int additionIndex = matchString.IndexOf("+");
        if(additionIndex < 0 && matchString.Contains("-")){      // If there is no + operator, try for -
            if( matchString.Contains("*-") ){
                // If the string contains "*-", search starting from beyond that point
                additionIndex = matchString.IndexOf("-", matchString.IndexOf("*-")+1);
            }
            else{
                additionIndex = matchString.IndexOf("-");
            }            
        }
        if(additionIndex > -1){
            flatAddition = GetNumberFromMatchStringGivenIndices(additionIndex, -1, matchString);
        }

        // Set the starting value to the stat
        float totalValue = GetStartingValueFromStatCode(code);

        totalValue = (totalValue * (percent * 0.01f)) * multiplier + flatAddition;

        // Set the color of the text according to if it is pos or neg
        string startColor = "";
        string endColor = "</color>";
        if(totalValue > 0){
            startColor = "<color=" + InGameUIManager.slimeGreenColor + ">";
        }
        else{
            startColor = "<color=" + InGameUIManager.magentaColor + ">";
        }

        return startColor + Mathf.Abs(totalValue) + endColor;
    }

    private float GetNumberFromMatchStringGivenIndices(int startingIndex, int commaIndex, string matchString)
    {
        int valueIndex = startingIndex + 1;
        int length = commaIndex - valueIndex;
        if(commaIndex < 0){
            length = matchString.IndexOf("}") - valueIndex;
        }
        System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
        return float.Parse(matchString.Substring(valueIndex,length), styles);
    }

    private DescriptionVariableCode GetCodeFromString(string matchString)
    {
        // Loop through all possible codes; if there's a match, return it
        for(int i = 0; i < (int)DescriptionVariableCode.enumSize; i++){
            if(matchString.Contains( ((DescriptionVariableCode)i).ToString() )){
                return (DescriptionVariableCode)i;
            }
        }
        Debug.LogError("No code match found for description variable: " + matchString);
        return DescriptionVariableCode.enumSize;
    }

    private float GetStartingValueFromStatCode(DescriptionVariableCode code)
    {
        switch(code){
            // Primary Stats
            case DescriptionVariableCode.STR:
                return Player.instance.stats.Strength();
            case DescriptionVariableCode.DEX:
                return Player.instance.stats.Dexterity();
            case DescriptionVariableCode.INT:
                return Player.instance.stats.Intelligence();
            case DescriptionVariableCode.WIS:
                return Player.instance.stats.Wisdom();
            case DescriptionVariableCode.CHA:
                return Player.instance.stats.Charisma();
            case DescriptionVariableCode.CON:
                return Player.instance.stats.Constitution();

            // Damage Values
            case DescriptionVariableCode.SD:
                return Player.instance.stats.getSTRDamage();
            case DescriptionVariableCode.DD:
                return Player.instance.stats.getDEXDamage();
            case DescriptionVariableCode.ID:
                return Player.instance.stats.getINTDamage();
            case DescriptionVariableCode.WD:
                return Player.instance.stats.getWISDamage();

            // Secondary Stats
            case DescriptionVariableCode.ATS:
                return Player.instance.stats.getAttackSpeed();
            case DescriptionVariableCode.MOS:
                return Player.instance.stats.getMoveSpeed();
            case DescriptionVariableCode.DEF:
                return Player.instance.stats.getDefense();
            case DescriptionVariableCode.DOC:
                return Player.instance.stats.getDodgeChance();
            case DescriptionVariableCode.CRC:
                return Player.instance.stats.getCritChance();
            case DescriptionVariableCode.CRD:
                return Player.instance.stats.getCritDamage();
            case DescriptionVariableCode.TDR:
                return Player.instance.stats.getTrapDamageResist();
            case DescriptionVariableCode.HAS:
                return Player.instance.stats.getHaste();
        }
        Debug.LogError("No stat found for: " + code);
        return -1;
    }
}

/*
    TODO:
    - include the primary line from THAT item (i.e., if STR sword w/ primary line of +10% STR Damage, factor in that primary line to the sword damage description)
    - do NOT factor in your CURRENT buff from that item slot (if currently using a STR sword, don't factor in that buff)
*/


/*
    Description Variables
    =====================

    Valid Formats:
    --------------
    {CHA:85}                    ->  StatCode:%Value
    {WIS}                       ->  StatCode    (gets the flat value)
    {INT:0,*2,+5}               ->  * provides a multiplier value, + or - a flat addition or subtraction value
    {INT:0,*2}  {INT:0,+5}      ->  You CAN use just one or the other of * or -
    {DEF,+5}    {ATS,*44}       ->  You CAN get the flat StatCode value and add/subtract/multiply to it
    {WIS:0,*-10}                ->  You CAN multiply by a negative number
    {ATS:0,*0.21,+.54}          ->  Decimals okay, w/ or w/out a leading digit

    Invalid Formats:
    ----------------
    {WIS:0,*+10}                ->  CANNOT specify positive for a multiplier
    {DEX:50,}   {DEX:50,1}      ->  No stray commas; can't specify a number without an operator
    {INT:0,+2,*5}               ->  * MUST come BEFORE + or -
    {ATS,-*44}  {ATS,+*44}      ->  * cannot come after - or + in consecutive positions
    ++, --, +-, -+, **          ->  No consecutive duplicate operators
    {INT:0,-2,-5}               ->  ... Or non-consecutive duplicate operators (- and + count as the same type, so could not do - then + or vice versa)
*/