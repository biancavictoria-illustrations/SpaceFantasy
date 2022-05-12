using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EncounterRoomClose : MonoBehaviour
{
    public List<GameObject> forceFields;
    public Beetle boi;

    [SerializeField] private Room room;
    [SerializeField] private EnemyGen enemyGen;
    [SerializeField] private Color newColor;
    [SerializeField] private Light sceneLight;
    [SerializeField] private bool isBossRoom;

    private Color oldColor;
    private GameObject elevator;
   
    void Start()
    {
        if(GameManager.instance.InSceneWithRandomGeneration()){
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(StartOnGenerationComplete);
        }
        else{
            StartOnGenerationComplete();
        }
    }

    private void StartOnGenerationComplete()
    {
        if(isBossRoom)
        {
            elevator = FindObjectOfType<SceneTransitionDoor>().gameObject;
            sceneLight = FindObjectOfType<Light>();
        }
    }

    public void AddForceField(GameObject forceField)
    {
        if(forceFields == null)
            forceFields = new List<GameObject>();

        forceFields.Add(forceField);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(!GameManager.generationComplete){
                return;
            }

            // Enables the force field object
            foreach(GameObject ff in forceFields)
                ff.SetActive(true);

            if(isBossRoom)
            {
                oldColor = sceneLight.color;
                sceneLight.color = newColor;

                Beetle boi = FindObjectOfType<Beetle>();
                if(boi != null)
                {
                    boi.gameObject.SetActive(true);
                    boi.canAttack = true;
                }
            }
            else
            {
                enemyGen.spawnEnemies();
            }

            foreach(EntityHealth e in room.GetEnemyList())
            {
                e.OnDeath.AddListener(RoomOpen);
                if( isBossRoom && e.isBossEnemy ){
                    InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(true, e.enemyID);
                }
                Debug.Log("Enemy Added");
            }
        }
    }

    private void RoomOpen(EntityHealth _)
    {
        Debug.Log("Enemy Died");
        if (!room.hasEnemies())
        {
            foreach(GameObject ff in forceFields)
                ff.SetActive(false);
            
            if(isBossRoom)
            {
                elevator.GetComponent<Collider>().enabled = true;

                sceneLight.color = oldColor;
                Debug.Log("Room Open");
            }

            Debug.Log("disable encounter room");
            gameObject.SetActive(false);
        }
    }

}
