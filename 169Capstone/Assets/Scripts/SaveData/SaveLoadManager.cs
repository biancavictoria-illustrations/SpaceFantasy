using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadManager
{
    public static void SaveGame(int saveSlotNum, GameManager gameManager, PlayerInventory playerInventory, DialogueManager dialogueManager, StoryManager storyManager, PermanentUpgradeManager permanentUpgradeManager, JournalContentManager jcm) // Pass in the necessary values
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = GetSaveFilePath(saveSlotNum);
        FileStream stream = new FileStream(path, FileMode.Create);

        Save saveData = new Save(gameManager, playerInventory, dialogueManager, storyManager, permanentUpgradeManager, jcm); // Pass in the necessary values
        
        bf.Serialize(stream, saveData);
        stream.Close();
        
        // Debug.Log("Game Saved in Save Slot: " + saveSlotNum);
    }

    public static Save LoadGame(int saveSlotNum)
    {
        string path = GetSaveFilePath(saveSlotNum);

        // If there is a save file, retrieve it
        if (File.Exists(path)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            
            Save saveData = bf.Deserialize(stream) as Save;
            stream.Close();

            // Debug.Log("Game Loaded from Save Slot: " + saveSlotNum);
            return saveData;
        }
        else{
            // Debug.LogError("Save file not found in: " + path);
            return null;
        }
    }

    private static string GetSaveFilePath(int saveSlotNum)
    {
        return Application.persistentDataPath + "/gamesave" + saveSlotNum + ".save";
    }
}
