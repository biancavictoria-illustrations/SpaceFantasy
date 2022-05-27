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

    // TODO: Activate force fields on generation complete?

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
        foreach(GameObject ff in forceFields)
            ff.SetActive(false);
    }

    private void StartOnGenerationComplete()
    {
        if(!GameManager.generationComplete){
            return;
        }
        // Enables the force field object
        foreach(GameObject ff in forceFields)
            ff.SetActive(true);

        if(GameManager.instance.currentRunNumber == 1)
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
