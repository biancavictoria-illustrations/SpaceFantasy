using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalContentManager : MonoBehaviour
{
    public static JournalContentManager instance;

    public Dictionary<JournalContentDisplay.JournalContentID, JournalContent> contentDatabase {get; private set;}

    // Start is called before the first frame update
    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }

        contentDatabase = new Dictionary<JournalContentDisplay.JournalContentID, JournalContent>();
        LoadAllJournalContentObjects();
    }

    private void LoadAllJournalContentObjects()
    {
        // ""
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
