using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomForceFields : MonoBehaviour
{
    [HideInInspector] public List<GameObject> forceFields = new List<GameObject>();
    [SerializeField] private Room room;
    [SerializeField] private GameObject startBow;
    [SerializeField] private GameObject CaptainsLog;
    [SerializeField] private GameObject swordLight;

    void Start()
    {
        if(GameManager.instance.InSceneWithRandomGeneration())
        {
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(StartOnGenerationComplete);
        }
        else
        {
            StartOnGenerationComplete();
        }
    }
    public void RoomOpenOnObjectInteracted()
    {
        foreach(GameObject ff in forceFields){
            ff.SetActive(false);
            ff.GetComponent<SFXTrigger>().PlaySecondarySFX();
        }
    }

    private void StartOnGenerationComplete()
    {
        if(!GameManager.generationComplete){
            return;
        }
        // Enables the force field object
        foreach(GameObject ff in forceFields){
            ff.SetActive(true);
            // Don't play the SFX here because they should be up from the start
        }

        if(GameManager.instance.currentRunNumber == 1 && GameManager.instance.currentSceneName == GameManager.GAME_LEVEL1_STRING_NAME)
        {
            startBow.SetActive(false);
            CaptainsLog.SetActive(true);
            swordLight.SetActive(false);
        }
    }

    public void AddForceField(GameObject forceField)
    {
        if(forceFields == null)
            forceFields = new List<GameObject>();

        forceFields.Add(forceField);
    }
}
