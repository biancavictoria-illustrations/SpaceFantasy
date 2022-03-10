using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterRoomClose : MonoBehaviour
{
    [SerializeField] private Room room;
    [SerializeField] private Color newColor;
    [SerializeField] private Light sceneLight;
    [SerializeField] private bool isBossRoom;
    [SerializeField] private List<GameObject> forceFields;

    private Color oldColor;
    private GameObject elevator;
   
    void Start()
    {
        if(isBossRoom)
            elevator = FindObjectOfType<SceneTransitionDoor>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Enables the force field object
            foreach(GameObject ff in forceFields)
                ff.SetActive(true);
            
            foreach(EntityHealth e in room.enemies)
            {
                e.OnDeath.AddListener(RoomOpen);
                if( isBossRoom && e.isBossEnemy ){
                    InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(true, e.enemyID);
                }
            }

            if(isBossRoom)
            {
                oldColor = sceneLight.color;
                sceneLight.color = newColor;

                Beetle boi = FindObjectOfType<Beetle>();
                if(boi != null)
                    boi.canAttack = true;
            }
        }
    }

    private void RoomOpen(EntityHealth _)
    {
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
