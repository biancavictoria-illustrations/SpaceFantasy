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
    [SerializeField] private GameObject bossPrefab;
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

        if(isBossRoom)
        {
            foreach(Light light in FindObjectsOfType<Light>())
            {
                if(light.type == LightType.Directional)
                {
                    sceneLight = light;
                    break;
                }
            }
        }
    }

    private void StartOnGenerationComplete()
    {
        if(isBossRoom)
        {
            elevator = FindObjectOfType<SceneTransitionDoor>().gameObject;
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

                GameObject boss = Instantiate(bossPrefab, room.transform.position, room.transform.rotation);
                boi = boss.GetComponent<Beetle>();
                boi.bossRoomScript = room;

                AudioManager.Instance.queueMusicAfterFadeOut(AudioManager.MusicTrack.BossMusic);
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
