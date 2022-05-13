using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalContentManager : MonoBehaviour
{
    public Dictionary<JournalContentDisplay.JournalContentID, JournalContent> contentDatabase {get; private set;}

    // Start is called before the first frame update
    void Awake()
    {
        contentDatabase = new Dictionary<JournalContentDisplay.JournalContentID, JournalContent>();
        LoadAllJournalContentObjects();
    }

    private void LoadAllJournalContentObjects()
    {
        // Load in crew page data
        Object[] journalContentList = Resources.LoadAll("JournalContent/Crew", typeof(JournalContent));
        foreach(Object c in journalContentList){
            JournalContent content = (JournalContent)c;
            if(contentDatabase.ContainsKey(content.InternalID())){
                continue;
            }
            contentDatabase.Add(content.InternalID(), content);
        }

        // "JournalContent/Locations"

        // "JournalContent/Stats"

        // "JournalContent/Combat"

        // "JournalContent/Gear"
    }
}
