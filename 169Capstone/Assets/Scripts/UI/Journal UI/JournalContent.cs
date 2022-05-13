using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalContent : ScriptableObject
{
    [Tooltip("INTERNAL ID for indexing and displaying the right info at the right time")]
    [SerializeField] protected JournalContentDisplay.JournalContentID internalID;

    [Tooltip("PLAYER FACING entry name")]
    [SerializeField] protected string entryName;
    [Tooltip("PLAYER FACING flavor ID number")]
    [SerializeField] protected string entryID;

    [SerializeField] protected Sprite profilePicture;

    [TextArea(15,20)]
    [SerializeField] protected string reportNotes;

    public JournalContentDisplay.JournalContentID InternalID()
    {
        return internalID;
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
