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
        LoadContentFromLocation("JournalContent/Crew");

        // TODO: the last type
        // "JournalContent/Locations"

        // Load in stat page data
        LoadContentFromLocation("JournalContent/Stats");

        // Load in enemy page data
        LoadContentFromLocation("JournalContent/Enemies");

        // Load in item page data
        LoadContentFromLocation("JournalContent/Items");
    }

    private void LoadContentFromLocation(string location)
    {
        Object[] journalContentList = Resources.LoadAll(location, typeof(JournalContent));
        foreach(Object c in journalContentList){
            JournalContent content = (JournalContent)c;
            if(contentDatabase.ContainsKey(content.InternalID())){
                continue;
            }
            contentDatabase.Add(content.InternalID(), content);
        }
    }
}
