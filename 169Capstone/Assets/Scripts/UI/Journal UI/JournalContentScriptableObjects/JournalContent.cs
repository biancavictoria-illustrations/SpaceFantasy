using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Each page has a UNIQUE ID
public enum JournalContentID{
    // CREW
    Stellan,
    Bryn,
    Rhian,
    Doctor,
    Captain,
    Orby,
    Atlan,  // Hide him, unlock him later maybe?

    // STATS
    STR,
    DEX,
    CON,
    INT,
    WIS,
    CHA,
    SecondaryStats,

    // ENEMIES
    SlimeDefault,
    SlimeElite,
    RobertDefault,
    RobertElite,
    CyberHarpy,
    BrutePest,
    Harvester,
    TimeLich,

    // LOCATIONS
    TheOrbis,
    Medbay,
    GearShop,
    WeaponsShop,
    MainHub,
    BeetleBossRoom,
    LichBossRoom,
    Galaxy,

    // ITEMS
    Electrum,
    StarShards,
    Longsword,
    Bow,
    NanoKnuckles,
    RayGun,
    WristRocket,
    MurphysClaw,
    QuantumKunai,
    HelmOfTheRam,
    HoloHUDGlasses,
    PropulsionHeels,
    TrousersOfFortitude,

    // For looping and stuff
    enumSize
}

public class JournalContent : ScriptableObject
{
    [Tooltip("INTERNAL ID for indexing and displaying the right info at the right time")]
    [SerializeField] protected JournalContentID internalID;

    [Tooltip("Set to true if this journal entry is locked on start; false if we can see it right away")]
    [SerializeField] protected bool lockedOnStart = false;

    [Tooltip("PLAYER FACING entry name")]
    [SerializeField] protected string entryName;
    [Tooltip("PLAYER FACING flavor ID number")]
    [SerializeField] protected string entryID;

    [SerializeField] protected Sprite profilePicture;

    [TextArea(15,20)]
    [SerializeField] protected string reportNotes;

    public JournalContentID InternalID()
    {
        return internalID;
    }

    public bool LockedOnStart()
    {
        return lockedOnStart;
    }

    public string EntryName()
    {
        return entryName;
    }

    public string EntryID()
    {
        return entryID;
    }

    public Sprite ProfilePicture()
    {
        return profilePicture;
    }

    public string ReportNotes()
    {
        return reportNotes;
    }
}
